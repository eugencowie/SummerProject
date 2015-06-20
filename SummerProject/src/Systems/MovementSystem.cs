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
    class MovementSystem : EntityComponentProcessingSystem<MoveAction, Transform>
    {
        GraphicsDevice graphics;

        AStar.AStar astar;
        int currentIndex;

        Point? previousDestination;

        SpriteBatch debugBatch;
        Texture2D debugTexture;

        public override void LoadContent()
        {
            graphics = BlackBoard.GetEntry<Game>("Game").GraphicsDevice;
            previousDestination = null;
            debugBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            debugTexture = BlackBoard.GetEntry<Game>("Game").Content.Load<Texture2D>("textures/selector");
            astar = null;
        }

        public override void Process(Entity entity, MoveAction moveAction, Transform transform)
        {
            Entity level = entityWorld.TagManager.GetEntity("level");

            Vector2 position = transform.Position;
            var destinationBlock = new Point((int)moveAction.Destination.X, (int)moveAction.Destination.Y);
            float speed = moveAction.Speed;

            Tilemap tilemap = level.GetComponent<Tilemap>();

            // Convert position and destination from pixel coords to block coords.
            Point positionBlock = Tilemap.PixelsToBlockCoords(position);


            #region Render debug texture

            // TODO: use the sprite rendering system for this?
            if (astar != null && astar.Path.Count > 0)
            {
                foreach (Vector2 node in astar.Path)
                {
                    var textureOrigin = new Vector2(debugTexture.Width / 2f, debugTexture.Height / 2f);
                    var destinationRect = new Rectangle {
                        X = (int)node.X * Constants.UnitSize,
                        Y = (int)node.Y * Constants.UnitSize,
                        Width = Constants.UnitSize,
                        Height = Constants.UnitSize,
                    };

                    debugBatch.Draw(
                        debugTexture,
                        destinationRect,
                        null,
                        Color.White,
                        0f,
                        textureOrigin,
                        SpriteEffects.None,
                        0.5f);
                }
            }

            #endregion


            if (previousDestination.HasValue && previousDestination.Value != destinationBlock)
                astar = null;

            previousDestination = destinationBlock;

            // Validate position is in grid.
            if (destinationBlock.X < 0 ||
                destinationBlock.Y < 0 ||
                destinationBlock.X > tilemap.Tiles.GetLength(0) - 1 ||
                destinationBlock.Y > tilemap.Tiles.GetLength(1) - 1)
            {
                astar = null;
                entity.RemoveComponent<MoveAction>();
                return;
            }

            if (astar == null)
            {
                AStar.TileInfo[,] tileInfo = tilemap.GetCollisionInfo();

                // Pass the tile information and a weight for the H
                // the lower the H weight value shorter the path
                // the higher it is the less number of checks it take to determine
                // a path
                astar = new AStar.AStar(tileInfo, 1, 100);
                astar.Start(positionBlock.X, positionBlock.Y, destinationBlock.X, destinationBlock.Y);

                // This particular implementation of the A* algorithm does not handle moving to adjacent
                // blocks consistently and will often not find a path, so if the path is empty and the
                // destination block is valid, see it the distance to the destination is less than two
                // blocks and, if so, create a new path to the destination.
                if (astar.Path.Count == 0 && tilemap.Tiles[destinationBlock.X, destinationBlock.Y].Collision.TileType == AStar.TileType.Floor)
                {
                    var positionVector = new Vector2(positionBlock.X, positionBlock.Y);
                    var destinationVector = new Vector2(destinationBlock.X, destinationBlock.Y);

                    Vector2 distance = destinationVector - positionVector;
                    if (distance.Length() < 2f)
                        astar.Path.Add(destinationVector);
                }

                currentIndex = 0;
            }

            if (astar.Path.Count == 0)
            {
                astar = null;
                entity.RemoveComponent<MoveAction>();
                return;
            }

            Vector2 newDestination = (astar.Path[currentIndex] * Constants.UnitSize);

            if (Math.Abs(newDestination.X - position.X) <= speed &&
                Math.Abs(newDestination.Y - position.Y) <= speed)
            {
                if (currentIndex == astar.Path.Count - 1)
                {
                    astar = null;
                    entity.RemoveComponent<MoveAction>();
                    return;
                }

                currentIndex++;
            }
            else
            {
                Vector2 direction = newDestination - position;
                direction.Normalize();

                position += direction * speed;
                transform.Position = position;
            }
        }
    }
}
