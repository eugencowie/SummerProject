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

            // Create the entity manager and store some useful variables to be accessed elsewhere.
            entityManager = new EntityWorld();
            EntitySystem.BlackBoard.SetEntry("Game", this);
            EntitySystem.BlackBoard.SetEntry("SpriteBatch", spriteBatch);
            EntitySystem.BlackBoard.SetEntry("Camera", camera);
            entityManager.InitializeAll(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create the level.
            Entity level = LevelEntity.BuildEntity(entityManager, "Tilemap.txt");

            // Get the player start position.
            SymbolicBlock[,] symbolic = level.GetComponent<TilemapComponent>().SymbolicBlocks;
            int blockSize = level.GetComponent<TilemapComponent>().BlockSize;
            Vector2 playerStart = Vector2.One;
            for (int y = 0; y < symbolic.GetLength(1); y++)
                for (int x = 0; x < symbolic.GetLength(0); x++)
                    if (symbolic[x, y] == SymbolicBlock.PlayerStart)
                        playerStart = new Vector2(x * blockSize, y * blockSize);

            // Create the block.
            Entity player = PlayerEntity.BuildEntity(entityManager, playerStart);
            camera.Position = player.GetComponent<TransformComponent>().Position;
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the entity manager.
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

            // Draw the entities.
            entityManager.Draw();

            // End the spritebatch.
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
