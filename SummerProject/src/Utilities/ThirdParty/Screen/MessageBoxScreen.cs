// ReSharper disable All

#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SummerProject
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        #region Fields

        string message;
        Texture2D gradientTexture;

        InputAction menuSelect;
        InputAction menuCancel;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialisation


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="includeUsageText">Lets the caller specify whether to include the standard 
        /// "A=ok, B=cancel" usage text prompt.</param>
        public MessageBoxScreen(string message, bool includeUsageText = true)
        {
            const string usageText = "\nA button, Space, Enter = ok" +
                                     "\nB button, Esc = cancel"; 
            
            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Space, Keys.Enter },
                true);
            menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Escape, Keys.Back },
                true);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ContentManager content = ScreenManager.Game.Content;
                gradientTexture = content.Load<Texture2D>("textures/gradient");
            }
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex) || input.IsLeftMouseButtonClicked())
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex) || input.IsRightMouseButtonClicked())
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3f);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2f;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle(
                (int)textPosition.X - hPad,
                (int)textPosition.Y - vPad,
                (int)textSize.X + hPad * 2,
                (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            Color colour = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, colour);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, colour);

            spriteBatch.End();
        }


        #endregion
    }
}
