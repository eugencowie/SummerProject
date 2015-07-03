namespace SummerProject
{
    class HostGameMenuScreen : MenuScreen
    {
        public HostGameMenuScreen()
            : base("Host Game")
        {
            NetworkingSystem.Server.CheckIsRunning();

            NetworkingSystem.Client.ConnectedToHost += ConnectedToHost;
            NetworkingSystem.Client.Connect("127.0.0.1", Constants.NetworkPort);
        }


        private void ConnectedToHost(object sender, ConnectedToHostEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, null, new GameplayScreen());
        }
    }
}
