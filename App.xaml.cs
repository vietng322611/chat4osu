namespace chat4osu
{
    public partial class App : Application
    {
        public webSocket sock = new webSocket();
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}