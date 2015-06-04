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
        enum GameState {
            MainMenu,
            Options,
            Playing,
        }

        GameState currentGameState = GameState.MainMenu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // GameState.MainMenu:
        Button playButton;

        // GameState.Playing:
        Camera camera;
        EntityWorld entityManager;

        Texture2D healthBar;
        Texture2D manaBar;

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

            // Load the HUD.
            healthBar = Content.Load<Texture2D>("textures/healthbar");
            manaBar = Content.Load<Texture2D>("textures/manabar");

            // Create the level entity.
            Entity level = entityManager.CreateEntity();
            level.Tag = "level";
            Tilemap levelTilemap = TilemapLoader.ReadMapFromFile("Content/maps/Map1.tmx", entityManager);
            level.AddComponent(levelTilemap);

            // Get the player start position from the level tilemap.
            Point? playerStartBlock = levelTilemap.FirstObjectBlockOfType(ObjectBlock.PlayerStart);
            if (!playerStartBlock.HasValue) playerStartBlock = new Point(1, 1);
            Vector2 playerStart = levelTilemap.BlockCoordsToPixels(playerStartBlock.Value);

            // Create the player entity.
            Entity player = entityManager.CreateEntity();
            player.Tag = "player";
            player.AddComponent(new PlayerInfo() { PlayerId = 1, LocalPlayer = true });
            player.AddComponent(new Transform() { Position = playerStart, Size = new Vector2(40, 40) });
            player.AddComponent(new Sprite() { Texture = Content.Load<Texture2D>("textures/objects/player"), LayerDepth = 0.0f });
            player.AddComponent(new Inventory());

            // Get mob spawn positions from the level tilemap.
            List<Point> mobSpawnBlocks = levelTilemap.AllObjectBlocksOfType(ObjectBlock.Mob);
            foreach (Point mobSpawn in mobSpawnBlocks)
            {
                Vector2 position = levelTilemap.BlockCoordsToPixels(mobSpawn);
                float rotation = levelTilemap.Tiles[mobSpawn.X, mobSpawn.Y].ObjectRotation;
                Vector2 size = new Vector2(40, 40);

                Texture2D texture = Content.Load<Texture2D>("textures/objects/enemy");
                SpriteEffects effect = levelTilemap.Tiles[mobSpawn.X, mobSpawn.Y].ObjectEffect;

                // Create an enemy.
                Entity enemy = entityManager.CreateEntity();
                enemy.Group = "enemies";
                enemy.AddComponent(new Transform() { Position = position, Rotation = rotation, Size = new Vector2(40, 40) });
                enemy.AddComponent(new Sprite() { Texture = texture, Effects = effect, LayerDepth = 0.0f });
                enemy.AddComponent(new Inventory());
            }

            // Center the camera on the player at the start.
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
                    spriteBatch.End();
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

                    // End the spritebatch.
                    spriteBatch.End();

                    // Draw the HUD.
                    PlayerInfo playerInfo = entityManager.TagManager.GetEntity("player").GetComponent<PlayerInfo>();
                    spriteBatch.Begin();
                    spriteBatch.Draw(manaBar, new Rectangle(630, 580, (int)(150 * playerInfo.Mana.Percentage), 15), Color.White);
                    spriteBatch.Draw(healthBar, new Rectangle(630, 560, (int)(150 * playerInfo.Health.Percentage), 15), Color.White);
                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
