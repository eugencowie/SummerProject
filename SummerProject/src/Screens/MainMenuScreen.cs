using Microsoft.Xna.Framework;

namespace SummerProject
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create menu entries.
            var joinGameMenuEntry = new MenuEntry("Join Game");
            var hostGameMenuEntry = new MenuEntry("Host Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            joinGameMenuEntry.Selected += JoinGameMenuEntrySelected;
            hostGameMenuEntry.Selected += HostGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(joinGameMenuEntry);
            MenuEntries.Add(hostGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        /// <summary>
        /// Event handler for when the Join Game menu entry is selected.
        /// </summary>
        private void JoinGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Client.Active = true;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Host Game menu entry is selected.
        /// </summary>
        private void HostGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Server.Active = true;
            Client.Active = true;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit to Windows?";

            var confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure you want
        /// to exit" message box.
        /// </summary>
        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
