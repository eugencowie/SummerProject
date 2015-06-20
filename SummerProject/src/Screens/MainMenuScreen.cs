using Microsoft.Xna.Framework;

namespace SummerProject
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialisation


        /// <summary>
        /// Constructor.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry joinGameMenuEntry = new MenuEntry("Join Game");
            MenuEntry hostGameMenuEntry = new MenuEntry("Host Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

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


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Join Game menu entry is selected.
        /// </summary>
        void JoinGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Client.Active = true;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Host Game menu entry is selected.
        /// </summary>
        void HostGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Server.Active = true;
            Client.Active = true;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
