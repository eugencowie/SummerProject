using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SummerProject
{
    /// <summary>
    /// The main game class.
    /// </summary>
    public class SummerProjectGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        IScreenFactory screenFactory;

        public SummerProjectGame()
        {
            // Set up XNA.
            graphics = new GraphicsDeviceManager(this) {
                SynchronizeWithVerticalRetrace = true,
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600,
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";

            // Set the mouse cursor to be visible.
            IsMouseVisible = true;

            // Create the screen factory and add it to the services.
            screenFactory = new BasicScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager and add it to the game class components.
            screenManager = new ScreenManager(this);
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
