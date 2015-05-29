using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SummerProject
{
    public class SummerProjectGame : Microsoft.Xna.Framework.Game
    {
        enum GameState {
            MainMenu,
            Options,
            Playing,
        }

        GameState currentGameState = GameState.MainMenu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // MainMenu
        Button playButton;

        // Playing
        Camera camera;
        EntityWorld entityManager;

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
        }

        protected override void Initialize()
        {
            // Create spritebatch and camera.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera();

            // Store some useful variables to be accessed elsewhere.
            EntitySystem.BlackBoard.SetEntry("Game", this);
            EntitySystem.BlackBoard.SetEntry("SpriteBatch", spriteBatch);
            EntitySystem.BlackBoard.SetEntry("Camera", camera);

            // Create the entity manager and initialise all systems.  It is important that
            // the initializeAll() function is called *after* all required BlackBoard entries
            // have been set.
            entityManager = new EntityWorld();
            entityManager.InitializeAll(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create the main menu buttons.
            playButton = new Button(Content.Load<Texture2D>("textures/button_play"), GraphicsDevice);
            playButton.SetPosition(new Vector2(350, 300));

            // Create the level.
            Entity level = entityManager.CreateEntity();
            level.Tag = "level";
            TilemapComponent levelTilemap = TilemapLoader.ReadMapFromFile("Content/maps/Map1.tmx");
            level.AddComponent(levelTilemap);

            // Get the player start position.
            Point? playerStartBlock = levelTilemap.FirstSymbolicBlockOfType(SymbolicBlock.PlayerStart);
            if (!playerStartBlock.HasValue) playerStartBlock = new Point(1, 1);
            Vector2 playerStart = levelTilemap.BlockCoordsToPixels(playerStartBlock.Value);

            // Create the player.
            Entity player = entityManager.CreateEntity();
            player.Tag = "player";
            player.AddComponent(new PlayerComponent() { PlayerId = 1, LocalPlayer = true });
            player.AddComponent(new TransformComponent() { Position = playerStart, Size = new Vector2(40, 40) });

            // Center the camera on the player.
            camera.Position = playerStart;
        }

        protected override void Update(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    // Update the buttons.
                    playButton.Update(Mouse.GetState());
                    if (playButton.isClicked == true)
                        currentGameState = GameState.Playing;
                    break;

                case GameState.Playing:
                    // Run the systems.
                    entityManager.Update();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    // Draw the buttons.
                    spriteBatch.Begin();
                    playButton.Draw(spriteBatch);
                    break;

                case GameState.Playing:
                    // Begin the spritebatch and apply the camera's transformation matrix.
                    spriteBatch.Begin(
                        SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null, null, null, null,
                        camera.GetTransformationMatrix(GraphicsDevice));
                    // Run the draw systems.
                    entityManager.Draw();
                    break;
            }

            // End the spritebatch.
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
