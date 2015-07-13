namespace SummerProject
{
    class HostGameMenuScreen : MenuScreen
    {
        public HostGameMenuScreen()
            : base("Host Game")
        {
            NetworkingSystem.Server.Start();

            NetworkingSystem.Client.Start();
            NetworkingSystem.Client.Connect("127.0.0.1", Constants.NetworkPort, () => {
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayScreen());
            });
        }
    }
}
