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

            // Try to load the map from a file, otherwise use the default map.
            int[,] blocks;
            try {
                blocks = TilemapLoader.ReadMapFromFile(file);
            }
            catch (Exception) {
                blocks = TilemapLoader.DefaultMap;
            }

            // The tilemap component contains tile data about the level.
            entity.AddComponent(new TilemapComponent() {
                Blocks = blocks,
                BlockSize = 40
            });

            // Generate a A* tile info array based on the level (TODO: this should possible be based on a separate collision map).
            AStar.TileInfo[,] collisionBlocks = new AStar.TileInfo[blocks.GetLength(0), blocks.GetLength(1)];
            for (int y = 0; y < collisionBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < collisionBlocks.GetLength(0); x++)
                {
                    collisionBlocks[x, y] = new AStar.TileInfo();

                    if (blocks[x, y] == 1)
                        collisionBlocks[x, y].TileType = AStar.TileType.Wall;
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
