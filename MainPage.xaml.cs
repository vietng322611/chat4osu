namespace chat4osu
{
    public partial class MainPage : ContentPage
    {
        private webSocket sock = ((App)Application.Current).sock;
        private Entry entry;
        private Label label;

        public string NICK { get; set; }

        public MainPage()
        {
            InitializeComponent();
            entry = (Entry)FindByName("Input");
            label = (Label)FindByName("Message");
            sock.Recv(label);
        }

        public void OnClicked(object sender, EventArgs e)
        {
            sock.Send(entry.Text, label);
            entry.Text = "";
        }

        public void OnCompleted(object sender, EventArgs e)
        {
            sock.Send(entry.Text, label);
            entry.Text = "";
        }
    }
}