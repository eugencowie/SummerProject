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
        MenuEntry resolutionMenuEntry;
        MenuEntry fullscreenMenuEntry;
        MenuEntry vsyncMenuEntry;

        List<Point> resolutions;
        int currentResolution;

        bool fullscreen;
        bool vsync;


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create menu entries.
            resolutionMenuEntry = new MenuEntry(string.Empty);
            fullscreenMenuEntry = new MenuEntry(string.Empty);
            vsyncMenuEntry = new MenuEntry(string.Empty);
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
        /// Called when the screen is added to the screen manager or if the game resumes
        /// from being paused.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            // Get list of supported resolutions.
            resolutions = new List<Point>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                var resolution = new Point(mode.Width, mode.Height);
                if (!resolutions.Contains(resolution))
                    resolutions.Add(resolution);
            }

            // Add current resolution to list of resolutions if it does not exist.
            var graphics = ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            Point? current = null;
            if (graphics != null)
            {
                current = new Point(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                if (!resolutions.Contains(current.Value))
                    resolutions.Add(current.Value);
            }

            // Order the list of resolutions from smallest to largest total pixel count.
            resolutions = resolutions.OrderBy(p => p.X * p.Y).ToList();

            // Set current options.
            currentResolution = (current.HasValue ? resolutions.IndexOf(current.Value) : 0);
            fullscreen = (graphics != null ? graphics.IsFullScreen : Options.Instance.Fullscreen);
            vsync = (graphics != null ? graphics.SynchronizeWithVerticalRetrace : Options.Instance.VSync);

            // Set menu entry text.
            SetMenuEntryText();
        }


        /// <summary>
        /// Fills in the latest values for the menu entries.
        /// </summary>
        private void SetMenuEntryText()
        {
            Point res = resolutions[currentResolution];
            resolutionMenuEntry.Text = string.Format("Resolution: {0}x{1}", res.X, res.Y);

            fullscreenMenuEntry.Text = "Fullscreen: " + (fullscreen ? "on" : "off");
            vsyncMenuEntry.Text = "VSync: " + (vsync ? "on" : "off");
        }


        /// <summary>
        /// Event handler for when the user has cancelled the menu.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // Apply new options.
            var graphics = ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            if (graphics != null)
            {
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
            Options.WriteOptionsToFile(Constants.OptionsFile);

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
