using Artemis;
using System;

namespace SummerProject
{
    static class LevelEntity
    {
        public static Entity BuildEntity(EntityWorld entityManager, string file)
        {
            Entity entity = entityManager.CreateEntity();
            entity.Tag = "level";

            // The renderable component tells the rendering system to render the level.
            entity.AddComponent(new RenderableComponent());

            // Try to load the map from a file.
            TilemapComponent tilemap = TilemapLoader.ReadMapFromFile(file);

            // The tilemap component contains tile data about the level.
            entity.AddComponent(tilemap);

            // Generate a A* tile info array based on the level (TODO: this should possible be based on a separate collision map).
            AStar.TileInfo[,] collisionBlocks = new AStar.TileInfo[tilemap.VisualBlocks.GetLength(0), tilemap.VisualBlocks.GetLength(1)];
            for (int y = 0; y < collisionBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < collisionBlocks.GetLength(0); x++)
                {
                    collisionBlocks[x, y] = new AStar.TileInfo();

                    switch (tilemap.VisualBlocks[x, y])
                    {
                        case VisualBlock.Wall:
                        case VisualBlock.UnpassableGround:
                        case VisualBlock.LockedGround:
                        case VisualBlock.LockedDoor:
                            collisionBlocks[x, y].TileType = AStar.TileType.Wall;
                            break;
                    }
                }
            }     

            // The level collision component contains the A* tile info array which is used for pathfinding.
            entity.AddComponent(new LevelCollisionComponent() {
                Blocks = collisionBlocks
            });

            return entity;
        }
    }
}
