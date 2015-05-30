using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    class MovementSystem : EntityComponentProcessingSystem<PlayerMoveAction, TransformComponent>
    {
        GraphicsDevice graphics;

        AStar.AStar a_star;
        int currentIndex;

        Point? previousDestination;

        SpriteBatch debugBatch;
        Texture2D debugTexture;

        public override void LoadContent()
        {
            graphics = EntitySystem.BlackBoard.GetEntry<Game>("Game").GraphicsDevice;
            previousDestination = null;
            debugBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            debugTexture = EntitySystem.BlackBoard.GetEntry<Game>("Game").Content.Load<Texture2D>("textures/selector");
            a_star = null;
        }

        public override void Process(Entity entity, PlayerMoveAction goToLocationAction, TransformComponent transformComponent)
        {
            Entity level = entityWorld.TagManager.GetEntity("level");

            Vector2 position = transformComponent.Position;
            Point destinationBlock = new Point((int)goToLocationAction.Destination.X, (int)goToLocationAction.Destination.Y);
            float speed = goToLocationAction.Speed;

            TilemapComponent tilemapComponent = level.GetComponent<TilemapComponent>();
            VisualBlock[,] blocks = tilemapComponent.VisualBlocks;
            int blockSize = tilemapComponent.BlockSize;

            // Convert position and destination from pixel coords to block coords.
            Point positionBlock = new Point() {
                X = (int)Math.Round(position.X / blockSize),
                Y = (int)Math.Round(position.Y / blockSize),
            };


            #region Render debug texture

            if (a_star != null && a_star.Path.Count > 0)
            {
                foreach (Vector2 node in a_star.Path)
                {
                    Vector2 textureOrigin = new Vector2(debugTexture.Width / 2.0f, debugTexture.Height / 2.0f);
                    Rectangle destinationRect = new Rectangle()
                    {
                        X = (int)node.X * blockSize,
                        Y = (int)node.Y * blockSize,
                        Width = blockSize,
                        Height = blockSize,
                    };

                    debugBatch.Draw(
                        debugTexture,
                        destinationRect,
                        null,
                        Color.White,
                        0.0f,
                        textureOrigin,
                        SpriteEffects.None,
                        0.5f);
                }
            }

            #endregion


            if (previousDestination.HasValue && previousDestination.Value != destinationBlock)
                a_star = null;

            previousDestination = destinationBlock;

            // Validate position is in grid.
            if (destinationBlock.X < 0 ||
                destinationBlock.Y < 0 ||
                destinationBlock.X > blocks.GetLength(0) - 1 ||
                destinationBlock.Y > blocks.GetLength(1) - 1)
            {
                a_star = null;
                entity.RemoveComponent<PlayerMoveAction>();
                return;
            }

            if (a_star == null)
            {
                AStar.TileInfo[,] tileInfo = tilemapComponent.CollisionBlocks;

                // Pass the tile information and a weight for the H
                // the lower the H weight value shorter the path
                // the higher it is the less number of checks it take to determine
                // a path
                a_star = new AStar.AStar(tileInfo, 1, 100);
                a_star.Start(positionBlock.X, positionBlock.Y, destinationBlock.X, destinationBlock.Y);

                if (a_star.Path.Count == 0)
                {
                    // Special case for moving to adjacent blocks.
                    Vector2 positionVector = new Vector2(positionBlock.X, positionBlock.Y);
                    Vector2 destinationVector = new Vector2(destinationBlock.X, destinationBlock.Y);
                    Vector2 distance = destinationVector - positionVector;
                    if (distance.Length() < 2.0f)
                        a_star.Path.Add(destinationVector);
                }

                currentIndex = 0;
            }

            if (a_star.Path.Count == 0)
            {
                a_star = null;
                entity.RemoveComponent<PlayerMoveAction>();
                return;
            }

            Vector2 newDestination = (a_star.Path[currentIndex] * blockSize);

            if (Math.Abs(newDestination.X - position.X) <= speed &&
                Math.Abs(newDestination.Y - position.Y) <= speed)
            {
                if (currentIndex == a_star.Path.Count - 1)
                {
                    a_star = null;
                    entity.RemoveComponent<PlayerMoveAction>();
                    return;
                }

                currentIndex++;
            }
            else
            {
                Vector2 direction = newDestination - position;
                direction.Normalize();

                position += direction * speed;
                transformComponent.Position = position;
            }
        }
    }
}
