// ReSharper disable All

#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SummerProject
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields


        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;


        #endregion

        #region Properties


        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        public Vector2 Position { get; set; }


        /// <summary>
        /// Can be used to store some information as part of the menu entry.
        /// </summary>
        public object UserData { get; set; }


        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        /// <summary>
        /// Event raised when the menu entry is increased.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Increased;


        /// <summary>
        /// Method for raising the Increased event.
        /// </summary>
        protected internal virtual void OnIncreaseEntry(PlayerIndex playerIndex)
        {
            if (Increased != null)
                Increased(this, new PlayerIndexEventArgs(playerIndex));
        }


        /// <summary>
        /// Event raised when the menu entry is decreased.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Decreased;


        /// <summary>
        /// Method for raising the Decreased event.
        /// </summary>
        protected internal virtual void OnDecreaseEntry(PlayerIndex playerIndex)
        {
            if (Decreased != null)
                Decreased(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        #region Initialisation


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
        {
            Text = text;
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1f);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0f);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2f);

            spriteBatch.DrawString(font, Text, Position, color, 0, origin, scale, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }


        #endregion
    }
}
