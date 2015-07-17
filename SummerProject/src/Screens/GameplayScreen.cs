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

        Camera camera;
        EntityWorld entityManager;

        bool freeCamera;

        InputAction pauseAction;
        float pauseAlpha;


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                camera = new Camera();
                entityManager = new EntityWorld();

                // Tell the networking system what to do if we lose connection to the server.
                NetworkingSystem.Client.OnLostConnectionToServer(() => {
                    LoadingScreen.Load(ScreenManager, false, ControllingPlayer,
                        new BackgroundScreen(),
                        new MainMenuScreen(),
                        new JoinGameMenuScreen());
                });

                // Store some useful variables to be accessed elsewhere.
                EntitySystem.BlackBoard.SetEntry("EntityWorld", entityManager);
                EntitySystem.BlackBoard.SetEntry("Game", ScreenManager.Game);
                EntitySystem.BlackBoard.SetEntry("SpriteBatch", ScreenManager.SpriteBatch);
                EntitySystem.BlackBoard.SetEntry("Content", content);
                EntitySystem.BlackBoard.SetEntry("Camera", camera);

                // Initialise all systems. It is important that the InitializeAll function
                // is called *after* all of the required BlackBoard entries have been set.
                entityManager.InitializeAll(true);

                // Create the level entity.
                Entity level = entityManager.CreateEntity(tag: "level");
                Tilemap levelTilemap = TilemapLoader.ReadMapFromFile("Content/maps/Map1.tmx", entityManager);
                level.AddComponent(levelTilemap);

                // Get the player start position from the level tilemap.
                Point? playerStart = levelTilemap.FirstObjectBlockOfType(ObjectBlock.PlayerStart);
                if (!playerStart.HasValue) playerStart = new Point(1, 1);

                // Create the player entity.
                NetworkingSystem.Client.RequestUniquePlayerId(id => {
                    entityManager.CreateEntity("players", "player1")
                        .AddPlayerComponents(content, id, playerStart.Value.ToVector2(), true);
                    NetworkingSystem.Client.PlayerCreated(id, playerStart.Value.ToVector2());
                });

                // Create any existing remote players.
                NetworkingSystem.Client.RequestWorldState((id, position) => {
                    if (id != entityManager.TagManager.GetEntity("player1").GetComponent<PlayerInfo>().PlayerId) {
                        entityManager.CreateEntity(group: "players")
                            .AddPlayerComponents(content, id, position, false);
                    }
                });

                // Get mob spawn positions from the level tilemap.
                foreach (Point position in levelTilemap.AllObjectBlocksOfType(ObjectBlock.Mob))
                {
                    float rotation = levelTilemap.Tiles[position.X, position.Y].ObjectRotation;
                    SpriteEffects effects = levelTilemap.Tiles[position.X, position.Y].ObjectEffect;

                    // Create an enemy.
                    entityManager.CreateEntity("enemies")
                        .AddEnemyComponents(content, position.ToVector2(), rotation, effects);
                }

                // Center the camera on the player at the start.
                camera.Position = playerStart.Value.ToVector2() * Constants.UnitSize;

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


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            NetworkingSystem.Client.OnLostConnectionToServer(null);
            NetworkingSystem.Client.Stop();
            NetworkingSystem.Server.Stop();
            content.Unload();
        }


        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            // Run the systems.
            entityManager.Update();
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
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


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            if (!ControllingPlayer.HasValue) return;
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all.
            bool gamePadDisconnected = (!gamePadState.IsConnected && input.GamePadWasConnected[playerIndex]);

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Get the player info.
                Entity entity = entityManager.TagManager.GetEntity("player1");
                PlayerInfo playerInfo = entity.GetComponent<PlayerInfo>();

                // Get the screen viewport.
                Viewport viewport = EntitySystem.BlackBoard.GetEntry<Game>("Game").GraphicsDevice.Viewport;

                // Get the camera and player transform.
                Transform playerTransform = entity.GetComponent<Transform>();
                Camera camera = EntitySystem.BlackBoard.GetEntry<Camera>("Camera");

                #region Camera movement

                MouseState mouse = input.CurrentMouseState;

                // Switch between free camera and locked to player.
                if (input.IsKeyClicked(Keys.Space, ControllingPlayer, out player))
                    freeCamera = !freeCamera;

                if (!freeCamera)
                {
                    // Lock camera to player.
                    camera.Position = playerTransform.Position * Constants.UnitSize;
                }
                else
                {
                    // Camera movement keyboard controls.
                    if (input.IsKeyDown(Keys.W, ControllingPlayer, out player))
                        camera.Position.Y -= 1 * (10 - camera.Zoom);
                    if (input.IsKeyDown(Keys.S, ControllingPlayer, out player))
                        camera.Position.Y += 1 * (10 - camera.Zoom);
                    if (input.IsKeyDown(Keys.A, ControllingPlayer, out player))
                        camera.Position.X -= 1 * (10 - camera.Zoom);
                    if (input.IsKeyDown(Keys.D, ControllingPlayer, out player))
                        camera.Position.X += 1 * (10 - camera.Zoom);

                    // Check the mouse in within the bounds of the window...
                    if (mouse.X >= 0 && mouse.X <= viewport.Width &&
                        mouse.Y >= 0 && mouse.Y <= viewport.Height)
                    {
                        // Camera movement mouse controls.
                        int screenEdgeBuffer = 60;
                        if (mouse.Y < screenEdgeBuffer)
                            camera.Position.Y -= 1 * (10 - camera.Zoom);
                        if (mouse.Y > viewport.Height - screenEdgeBuffer)
                            camera.Position.Y += 1 * (10 - camera.Zoom);
                        if (mouse.X < screenEdgeBuffer)
                            camera.Position.X -= 1 * (10 - camera.Zoom);
                        if (mouse.X > viewport.Width - screenEdgeBuffer)
                            camera.Position.X += 1 * (10 - camera.Zoom);
                        if (input.IsMouseWheelScolledUp())
                            camera.Zoom += 0.1f;
                        if (input.IsMouseWheelScrolledDown())
                            camera.Zoom -= 0.1f;
                    }

                    // Camera zoom.
                    if (input.IsKeyDown(Keys.LeftShift, ControllingPlayer, out player))
                        camera.Zoom += 0.01f;
                    if (input.IsKeyDown(Keys.LeftControl, ControllingPlayer, out player))
                        camera.Zoom -= 0.01f;
                }

                #endregion

                // TODO: remove this from final version.
                if (input.IsKeyDown(Keys.F5, ControllingPlayer, out player))
                    playerInfo.Health.Current -= 1;
                if (input.IsKeyDown(Keys.F6, ControllingPlayer, out player))
                    playerInfo.Health.Current += 1;

                // Normalise the mouse coords so that (0,0) is the center instead of the top left.
                var mousePos = new Vector2 {
                    X = mouse.X - (viewport.Width / 2f),
                    Y = mouse.Y - (viewport.Height / 2f)
                };

                Vector2 playerPixels = playerTransform.Position * Constants.UnitSize;

                // Figure out the destination (in pixels).
                var destination = new Vector2 {
                    X = playerPixels.X + (camera.Position.X - playerPixels.X) + (mousePos.X * (1f / camera.Zoom)),
                    Y = playerPixels.Y + (camera.Position.Y - playerPixels.Y) + (mousePos.Y * (1f / camera.Zoom))
                };

                // Make player always face the mouse pointer.
                Vector2 direction = playerPixels - destination;
                playerTransform.Rotation = (float)Math.Atan2(direction.Y, direction.X) - ((float)Math.PI / 2f);

                // Move the player when the right mouse button is clicked.
                if (input.IsRightMouseButtonClicked())
                {
                    // Convert destination from pixel coords to block coords.
                    var destinationBlock = new Vector2 {
                        X = (int)Math.Round(destination.X / Constants.UnitSize),
                        Y = (int)Math.Round(destination.Y / Constants.UnitSize),
                    };

                    // If the player is already moving, stop and go to the new destination instead.
                    entity.GetComponent<Pathfinder>().Destination = destinationBlock;
                    entity.GetComponent<Pathfinder>().Speed = 4f;

                    NetworkingSystem.Client.PlayerMoved(entity.GetComponent<PlayerInfo>().PlayerId, destinationBlock);
                }
            }
        }
    }
}
