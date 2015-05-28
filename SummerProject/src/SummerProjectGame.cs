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
            level.AddComponent(TilemapLoader.ReadMapFromFile("Tilemap.txt"));
            level.AddComponent(new RenderableComponent());

            // Get the player start position.
            SymbolicBlock[,] symbolic = level.GetComponent<TilemapComponent>().SymbolicBlocks;
            int blockSize = level.GetComponent<TilemapComponent>().BlockSize;
            Vector2 playerStart = Vector2.One;
            for (int y = 0; y < symbolic.GetLength(1); y++)
                for (int x = 0; x < symbolic.GetLength(0); x++)
                    if (symbolic[x, y] == SymbolicBlock.PlayerStart)
                        playerStart = new Vector2(x * blockSize, y * blockSize);

            // Create the player.
            Entity player = entityManager.CreateEntity();
            player.Tag = "player";
            player.AddComponent(new PlayerComponent() { PlayerId = 1, LocalPlayer = true });
            player.AddComponent(new TransformComponent() { Position = playerStart, Size = new Vector2(40, 40) });
            player.AddComponent(new RenderableComponent());

            // Center the camera on the player.
            camera.Position = player.GetComponent<TransformComponent>().Position;
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
