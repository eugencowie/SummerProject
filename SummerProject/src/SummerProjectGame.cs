using Microsoft.Xna.Framework;
using System;

namespace SummerProject
{
    /// <summary>
    /// The main XNA game class.
    /// </summary>
    class SummerProjectGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;


        /// <summary>
        /// Constructor.
        /// </summary>
        public SummerProjectGame()
        {
            // Read options file.
            Options.LoadOptionsFromFile(Constants.OptionsFile);

            // Set up XNA.
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth       = Options.Instance.Width,
                PreferredBackBufferHeight      = Options.Instance.Height,
                IsFullScreen                   = Options.Instance.Fullscreen,
                SynchronizeWithVerticalRetrace = Options.Instance.VSync
            };
            Content.RootDirectory = "Content";

            // Set the mouse cursor to be visible.
            IsMouseVisible = true;

#if DEBUG
            // Allow user to resize the window, for testing purposes.
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
#endif

            // Create the screen factory and add it to the services.
            var screenFactory = new BasicScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager and add it to the game class components.
            var screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate the initial screens.
            screenManager.AddScreen(new BackgroundScreen(), PlayerIndex.One);
            screenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
        }


        /// <summary>
        /// This method is called when it is time to render the frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen.
            GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        /// <summary>
        /// Event handler for when the window is resized. Makes sure that XNA updates the
        /// resolution to match the new window size.
        /// </summary>
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Remove this event handler, so we don't call it when we change the window size in here
            Window.ClientSizeChanged -= new EventHandler<EventArgs>(Window_ClientSizeChanged);

            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();

            // Add this event handler back.
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }
    }
}
