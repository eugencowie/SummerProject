using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SummerProject
{
    /// <summary>
    /// The background screen sits behind all the other menu screens. It draws a
    /// background image that remains fixed in place regardless of whatever transitions
    /// the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                // The background texture is quite big, so we use our own local ContentManager
                // to load it. This allows us to unload before going from the menus into the game
                // itself, wheras if we used the shared ContentManager provided by the Game class,
                // the content would remain loaded.
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                backgroundTexture = content.Load<Texture2D>("textures/background");
            }
        }

        public override void Unload()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Updates the background screen. Unlike most screens, this should not transition
            // off even if it has been covered by another screen. This overload forces the
            // coveredByOtherScreen parameter to false in order to stop the base Update method
            // wanting to transition off.
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            var colour = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, fullscreen, colour);
            spriteBatch.End();
        }
    }
}
