using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    public class SummerProjectGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
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
            // Run the systems.
            entityManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Begin the spritebatch and apply the camera's transformation matrix.
            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null, null, null,
                camera.GetTransformationMatrix(GraphicsDevice));

            // Run the draw systems.
            entityManager.Draw();

            // End the spritebatch.
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
