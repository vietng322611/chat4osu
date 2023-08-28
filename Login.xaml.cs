namespace chat4osu;

public partial class Login : ContentPage
{
	private webSocket sock = ((App)Application.Current).sock;

    public Login()
	{
		InitializeComponent();
	}
    public void OnPasswCompleted(object sender, EventArgs e)
    {
		login();
    }

    public void OnClicked(object sender, EventArgs e)
	{
        login();
	}

	public async void login()
	{
        string NICK = ((Entry)FindByName("UsernameEntry")).Text;
        string PASSW = ((Entry)FindByName("PasswEntry")).Text;

        sock.startClient(NICK, PASSW);

        if (sock.code == 0)
        {
            MainPage page = new MainPage();
            page.NICK = NICK;
            await Navigation.PushAsync(page);
        }
        else
        {
            Label label = (Label)FindByName("Status");
            label.Text = sock.error;
        }
    }
}