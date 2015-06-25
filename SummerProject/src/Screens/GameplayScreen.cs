using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SummerProject
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;

        Camera camera;
        EntityWorld entityManager;

        InputAction pauseAction;
        float pauseAlpha;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Escape },
                true);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("fonts/gamefont");

                camera = new Camera();

                // Store some useful variables to be accessed elsewhere.
                EntitySystem.BlackBoard.SetEntry("Game", ScreenManager.Game);
                EntitySystem.BlackBoard.SetEntry("SpriteBatch", ScreenManager.SpriteBatch);
                EntitySystem.BlackBoard.SetEntry("Camera", camera);

                // Create the entity manager and initialise all systems.  It is important that
                // the InitializeAll() function is called *after* all required BlackBoard entries
                // have been set.
                entityManager = new EntityWorld();
                entityManager.InitializeAll(true);

                // Create the level entity.
                Entity level = entityManager.CreateEntity(tag: "level");
                Tilemap levelTilemap = TilemapLoader.ReadMapFromFile("Content/maps/Map1.tmx", entityManager);
                level.AddComponent(levelTilemap);

                // Get the player start position from the level tilemap.
                Vector2? playerStart = levelTilemap.FirstObjectBlockOfType(ObjectBlock.PlayerStart);
                if (!playerStart.HasValue) playerStart = new Vector2(1f, 1f);

                // Create the player entity.
                entityManager.CreateEntity("players", "player1")
                    .AddPlayerComponents(content, playerStart.Value, true);

                // Get mob spawn positions from the level tilemap.
                foreach (Vector2 position in levelTilemap.AllObjectBlocksOfType(ObjectBlock.Mob))
                {
                    float rotation = levelTilemap.Tiles[(int)Math.Round(position.X), (int)Math.Round(position.Y)].ObjectRotation;
                    SpriteEffects effects = levelTilemap.Tiles[(int)Math.Round(position.X), (int)Math.Round(position.Y)].ObjectEffect;

                    // Create an enemy.
                    entityManager.CreateEntity("enemies")
                        .AddEnemyComponents(content, position, rotation, effects);
                }

                // Center the camera on the player at the start.
                camera.Position = Tilemap.BlockCoordsToPixels(playerStart.Value);

                // Simulate a longer loading time by delaying for a while, giving us
                // a chance to admire the beautiful loading screen.
                // TODO: remove this in final version
                //Thread.Sleep(1500);

                // Once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Unload()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            // TODO: Run the game when the window is not active?
#if !DEBUG
            if (IsActive)
#endif
            {
                // Run the systems.
                entityManager.Update();
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            /*
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = (!gamePadState.IsConnected && input.GamePadWasConnected[playerIndex]);

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 8f;
            }
            */
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin the spritebatch and apply the camera's transformation matrix.
            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                camera.GetTransformationMatrix(ScreenManager.GraphicsDevice));

            // Run the draw systems.
            entityManager.Draw();

            // End the spritebatch.
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0) {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2f);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
