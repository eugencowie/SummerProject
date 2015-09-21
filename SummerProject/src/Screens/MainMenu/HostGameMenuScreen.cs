namespace SummerProject
{
    class HostGameMenuScreen : MenuScreen
    {
        public HostGameMenuScreen()
            : base("Host Game")
        {
        }

        public override void Activate(bool instancePreserved)
        {
            int port = NetworkingSystem.Server.Start(Constants.NetworkPort, Constants.NetworkMaximumAttempts);

            NetworkingSystem.Client.Start();
            NetworkingSystem.Client.Connect("127.0.0.1", port, () => {
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayScreen());
            });
        }
    }
}
