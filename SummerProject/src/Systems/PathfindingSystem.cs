using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
#if DEBUG
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
#else
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
#endif
    class PathfindingSystem : EntityComponentProcessingSystem<Pathfinder, Transform>
    {
        SpriteBatch debugBatch;
        Texture2D debugTexture;

        public override void LoadContent()
        {
            debugBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            debugTexture = BlackBoard.GetEntry<ContentManager>("Content").Load<Texture2D>("textures/selector");
        }

        public override void Process(Entity entity, Pathfinder pathfinder, Transform transform)
        {
            #region Render debug texture

#if DEBUG

            if (pathfinder.CurrentPath != null &&
                pathfinder.CurrentPath.Path.Count > 0 &&
                pathfinder.CurrentIndex != pathfinder.CurrentPath.Path.Count)
            {
                foreach (Vector2 node in pathfinder.CurrentPath.Path)
                {
                    Vector2 textureOrigin = new Vector2(debugTexture.Width / 2f, debugTexture.Height / 2f);
                    Rectangle destinationRect = new Rectangle {
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

#endif

            #endregion

            Tilemap tilemap = entityWorld.TagManager.GetEntity("level").GetComponent<Tilemap>();

            // TODO: Make this framerate independent.
            float speed = pathfinder.Speed / 60f;

            // If the A* path gets set to null, it means that it should be recreated. This
            // might happen if the map's collision data changes.
            if (pathfinder.CurrentPath == null) {
                pathfinder.CurrentPath = new AStar.AStar(tilemap.GetCollisionInfo(), 1, 100);
                pathfinder.CurrentDestination = transform.Position;
                pathfinder.CurrentIndex = 0;
            }

            // Since the destination can be changed at any time, we need to check that
            // the A* path reflects the path to the current destination.
            if (pathfinder.Destination != pathfinder.CurrentDestination)
            {
                Point currentBlock = transform.Position.Round();
                Point destinationBlock = pathfinder.Destination.Round();

                // Validate position is in grid.
                if (destinationBlock.X < 0 ||
                    destinationBlock.Y < 0 ||
                    destinationBlock.X > tilemap.Tiles.GetLength(0) - 1 ||
                    destinationBlock.Y > tilemap.Tiles.GetLength(1) - 1)
                {
                    destinationBlock = currentBlock;
                }

                pathfinder.CurrentPath.Start(currentBlock.X, currentBlock.Y, destinationBlock.X, destinationBlock.Y);

                // This particular implementation of the A* algorithm does not handle moving to adjacent
                // blocks consistently and will often not find a path, so if the path is empty and the
                // destination block is valid then qw check if the distance to the destination is less
                // than two blocks and, if so, create a new path to the destination.
                if (pathfinder.CurrentPath.Path.Count == 0 &&
                    tilemap.Tiles[destinationBlock.X, destinationBlock.Y].Collision.TileType == AStar.TileType.Floor)
                {
                    Vector2 distance = pathfinder.Destination - transform.Position;
                    if (distance.Length() < 2f) {
                        pathfinder.CurrentPath.PathAvailable = true;
                        pathfinder.CurrentPath.Path.Clear();
                        pathfinder.CurrentPath.Path.Add(destinationBlock.ToVector2());
                    }
                }

                // No path could be found.
                if (pathfinder.CurrentPath.Path.Count == 0)
                    pathfinder.Destination = transform.Position;

                pathfinder.CurrentIndex = 0;
                pathfinder.CurrentDestination = pathfinder.Destination;
            }

            // Check that there is a valid path and that we have not reached the end of it.
            if (pathfinder.CurrentPath.Path.Count != 0 &&
                pathfinder.CurrentIndex != pathfinder.CurrentPath.Path.Count)
            {
                // If the distance between the player and the current path node is greater than
                // the value of speed, then move the player towards the current node. Otherwise,
                // if the player has reached the current node then move on to the next path node.
                Vector2 nodeDistance = pathfinder.CurrentPath.Path[pathfinder.CurrentIndex] - transform.Position;
                if (nodeDistance.Length() > speed)
                {
                    var direction = new Vector2(nodeDistance.X, nodeDistance.Y);
                    direction.Normalize();

                    transform.Position += direction * speed;
                }
                else
                {
                    pathfinder.CurrentIndex++;
                }
            }
        }
    }
}
