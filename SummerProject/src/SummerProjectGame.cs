using Microsoft.Xna.Framework;

namespace SummerProject
{
    /// <summary>
    /// The main XNA game class.
    /// </summary>
    class SummerProjectGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public SummerProjectGame()
        {
            // Read options file.
            Options.LoadOptionData(Constants.OptionsFile);

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
#endif

            // Create the screen factory and add it to the services.
            var screenFactory = new BasicScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager and add it to the game class components.
            var screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate the initial screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
    }
}
