using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SummerProject
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu screen, and gives
    /// the user a chance to configure the game.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry resolutionMenuEntry;
        MenuEntry fullscreenMenuEntry;
        MenuEntry vsyncMenuEntry;

        static List<Point> resolutions = new List<Point> {
            new Point(800, 600),
            new Point(1024, 768),
            new Point(1280, 720),
            new Point(1280, 800),
            new Point(1280, 1024),
            new Point(1360, 768),
            new Point(1366, 768),
            new Point(1440, 900),
            new Point(1600, 900),
            new Point(1680, 1050),
            new Point(1920, 1080),
            new Point(2560, 1440),
            new Point(3840, 2160),
        };
        int currentResolution;

        bool fullscreen;
        bool vsync;

        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Add current resolution to list of resolutions if it does not exist.
            var current = new Point(Options.Instance.Width, Options.Instance.Height);
            if (!resolutions.Contains(current))
                resolutions.Add(current);
            resolutions = resolutions.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

            // Set current options.
            currentResolution = resolutions.IndexOf(current);
            fullscreen = Options.Instance.Fullscreen;
            vsync = Options.Instance.VSync;

            // Create menu entries.
            resolutionMenuEntry = new MenuEntry(string.Empty);
            fullscreenMenuEntry = new MenuEntry(string.Empty);
            vsyncMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            resolutionMenuEntry.Selected += ResolutionMenuEntryIncreased;
            resolutionMenuEntry.Increased += ResolutionMenuEntryIncreased;
            resolutionMenuEntry.Decreased += ResolutionMenuEntryDecreased;
            fullscreenMenuEntry.Selected += FullscreenMenuEntrySelected;
            fullscreenMenuEntry.Increased += FullscreenMenuEntrySelected;
            fullscreenMenuEntry.Decreased += FullscreenMenuEntrySelected;
            vsyncMenuEntry.Selected += VsyncMenuEntrySelected;
            vsyncMenuEntry.Increased += VsyncMenuEntrySelected;
            vsyncMenuEntry.Decreased += VsyncMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(vsyncMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        private void SetMenuEntryText()
        {
            resolutionMenuEntry.Text = "Resolution: " + resolutions[currentResolution].X + "x" + resolutions[currentResolution].Y;
            fullscreenMenuEntry.Text = "Fullscreen: " + (fullscreen ? "on" : "off");
            vsyncMenuEntry.Text = "VSync: " + (vsync ? "on" : "off");
        }


        /// <summary>
        /// Event handler for when the user has cancelled the menu.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // Apply new options.
            var graphicsService = (IGraphicsDeviceService)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService));
            if (graphicsService is GraphicsDeviceManager)
            {
                var graphics = (graphicsService as GraphicsDeviceManager);

                graphics.PreferredBackBufferWidth = resolutions[currentResolution].X;
                graphics.PreferredBackBufferHeight = resolutions[currentResolution].Y;
                graphics.IsFullScreen = fullscreen;
                graphics.SynchronizeWithVerticalRetrace = vsync;
                graphics.ApplyChanges();
            }

            // Set new options.
            Options.Instance.Width = resolutions[currentResolution].X;
            Options.Instance.Height = resolutions[currentResolution].Y;
            Options.Instance.Fullscreen = fullscreen;
            Options.Instance.VSync = vsync;

            // Write options to file.
            Options.WriteOptionData(Constants.OptionsFile);

            base.OnCancel(playerIndex);
        }


        /// <summary>
        /// Event handler for when the Resolution menu entry is increased.
        /// </summary>
        private void ResolutionMenuEntryIncreased(object sender, PlayerIndexEventArgs e)
        {
            currentResolution = (currentResolution + 1) % resolutions.Count;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Resolution menu entry is decreased.
        /// </summary>
        private void ResolutionMenuEntryDecreased(object sender, PlayerIndexEventArgs e)
        {
            currentResolution = currentResolution - 1;

            if (currentResolution == -1)
                currentResolution = resolutions.Count - 1;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Fullscreen menu entry is selected.
        /// </summary>
        private void FullscreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            fullscreen = !fullscreen;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the VSync menu entry is selected.
        /// </summary>
        private void VsyncMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vsync = !vsync;

            SetMenuEntryText();
        }
    }
}
