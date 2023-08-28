using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace chat4osu
{
    public class webSocket
    {
        public string SERVER = "irc.ppy.sh";
        public int PORT = 6667;
        public string NICK;
        public string error = "";
        public int code = 0;

        private NetworkStream stream;
        private TcpClient irc;
        private StreamReader reader;
        private StreamWriter writer;
        private CancellationTokenSource cancelSource = new CancellationTokenSource();

        private string inputLine;
        private string channel = "";
        public async void startClient(string user, string password)
        {
            try
            {
                irc = new TcpClient(SERVER, PORT);

                user = user.Replace(" ", "_");

                stream = irc.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                await writer.WriteLineAsync("PASS " + password); writer.Flush();
                await writer.WriteLineAsync("NICK " + user); writer.Flush();

                string ret = reader.ReadLineAsync().Result;

                if (ret.Contains("Bad authentication token"))
                {
                    error = "Wrong password or username";
                    code = 1;
                }

                NICK = user;
            }
            catch (Exception e)
            {
                error = e.ToString();
                code = 2;
            }

            code = 0;
        }

        private string ProcessLine(string line)
        {
            string[] message = line.Split(' ');
            string[] trash = { "QUIT", "JOIN", "PART" };
            if (message.Length < 3) return "";
            foreach (string iter in trash)
            {
                if (message[1] == iter) return "";
            }
            if (message[1] != "PRIVMSG") return string.Join(" ", message);

            string name = message[0].Split("!")[0];
            name = name[1..];
            channel = message[2];
            string msg = string.Join(" ", message[3..]);
            msg = name + ": " + msg[1..];
            return msg;
        }

        public async void Recv(Label label)
        {
            while (true)
            {
                inputLine = reader.ReadLineAsync().Result;
                if (inputLine == null) { continue; }

                string[] splitInput = inputLine.Split("\n");

                foreach (string line in splitInput)
                {
                    if (line.Contains("PING"))
                    {
                        string[] pong = line.Split(" ");
                        await writer.WriteLineAsync("PONG " + pong[1]);
                        continue;
                    }
                    string msg = ProcessLine(line);
                    if (msg.Length < 1) continue;
                    label.Text += '\n' + msg;
                }

                await Task.Delay(100);
            }
        }

        private async void Join(string[] inputSplit, Label label)
        {
            string message = "JOIN " + inputSplit[1];
            channel = inputSplit[1];
            await writer.WriteLineAsync(message); writer.Flush();
            label.Text += "\n" + "Joined " + channel;
        }

        private async void Part(string[] inputSplit, Label label)
        {
            string message = "PART " + inputSplit[1] + '\n';
            channel = "";
            await writer.WriteLineAsync(message); writer.Flush();
            label.Text += "\n" + "Leaved " + inputSplit[1];
        }

        public async void Send(string message, Label label)
        {
            string[] inputSplit = message.Split(' ');

            if (inputSplit[0] == "/join" || inputSplit[0] == "/j")
            {
                Join(inputSplit, label);
                return;
            }

            if (inputSplit[0] == "/part" || inputSplit[0] == "/p")
            {
                Part(inputSplit, label);
                return;
            }

            string _message = "PRIVMSG " + channel + " " + message + '\n';
            await writer.WriteLineAsync(_message); writer.Flush();
            label.Text += "\n" + NICK + ": " + message;
        }

        public void Close()
        {
            reader.Close();
            writer.Close();
            irc.Close();
        }
    }
}
