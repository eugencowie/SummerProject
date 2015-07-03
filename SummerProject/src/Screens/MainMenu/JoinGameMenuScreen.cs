namespace SummerProject
{
    class JoinGameMenuScreen : MenuScreen
    {
        public JoinGameMenuScreen()
            : base("Join Game")
        {
            var back = new MenuEntry("Back");
            back.Selected += OnCancel;
            MenuEntries.Add(back);

            NetworkingSystem.Client.DiscoveryResponse += HostDiscovered;
            NetworkingSystem.Client.ConnectedToHost += ConnectedToHost;

            NetworkingSystem.Client.DiscoverLocalPeers(Constants.NetworkPort);
        }


        private void HostDiscovered(object sender, DiscoveryResponseEventArgs e)
        {
            var menuEntry = new MenuEntry(string.Format("{0} ({1}:{2})", e.HostName, e.HostEndPoint.Address, e.HostEndPoint.Port));
            menuEntry.Selected += ServerMenuEntrySelected;

            // Quick and dirty way of storing the host's details as part of the menu entry.
            menuEntry.UserData = e;

            // Insert the host menu entry just before the 'back' menu entry.
            MenuEntries.Insert(MenuEntries.Count - 1, menuEntry);
        }


        private void ServerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var menuEntry = sender as MenuEntry;
            if (menuEntry == null)
                return;
            
            var host = menuEntry.UserData as DiscoveryResponseEventArgs;
            if (host == null)
                return;

            NetworkingSystem.Client.Connect(host.HostEndPoint.Address.ToString(), host.HostEndPoint.Port);
        }


        private void ConnectedToHost(object sender, ConnectedToHostEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, null, new GameplayScreen());
        }
    }
}
