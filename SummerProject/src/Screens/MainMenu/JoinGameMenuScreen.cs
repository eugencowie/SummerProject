﻿using System.Net;

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

            NetworkingSystem.Client.Start();
            NetworkingSystem.Client.DiscoverLocalPeers(Constants.NetworkPort, (name, endpoint) => {
                var menuEntry = new MenuEntry(string.Format("{0} ({1}:{2})", name, endpoint.Address, endpoint.Port));
                menuEntry.Selected += ServerMenuEntrySelected;
                menuEntry.UserData = endpoint;                         // quick and dirty way of storing the host's details
                MenuEntries.Insert(MenuEntries.Count - 1, menuEntry);  // insert just before the 'back' menu entry
            });
        }


        private void ServerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var menuEntry = sender as MenuEntry;
            if (menuEntry == null)
                return;
            
            var endpoint = menuEntry.UserData as IPEndPoint;
            if (endpoint == null)
                return;

            NetworkingSystem.Client.Connect(endpoint.Address.ToString(), endpoint.Port, () => {
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayScreen());
            });
        }
    }
}
