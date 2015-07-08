namespace SummerProject
{
    class HostGameMenuScreen : MenuScreen
    {
        public HostGameMenuScreen()
            : base("Host Game")
        {
            NetworkingSystem.Server.Start();

            NetworkingSystem.Client.ConnectedToHost += ConnectedToHost;
            NetworkingSystem.Client.Start();
            NetworkingSystem.Client.Connect("127.0.0.1", Constants.NetworkPort);
        }


        private void ConnectedToHost(object sender, ConnectedToHostEventArgs e)
        {
            NetworkingSystem.Client.ConnectedToHost -= ConnectedToHost;
            LoadingScreen.Load(ScreenManager, true, null, new GameplayScreen());
        }
    }
}
