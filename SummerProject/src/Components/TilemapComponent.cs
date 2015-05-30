using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    enum VisualBlock {
        None,
        Ground,
        UnpassableGround,
        Wall,
    }

    enum SymbolicBlock {
        None,
        LockedDoor,
        PlayerStart,
        General,
        Boss,
        Mob,
        MobSpawn,
        Chest,
        Trap,
        HealthPack,
        Button,
        Key
    }

    class TilemapComponent : IComponent
    {
        public VisualBlock[,] VisualBlocks;
        public SymbolicBlock[,] SymbolicBlocks;
        public AStar.TileInfo[,] CollisionBlocks;
        public int[,] Rotations;
        public int BlockSize;

        public Point? FirstSymbolicBlockOfType(SymbolicBlock blockType)
        {
            for (int y = 0; y < SymbolicBlocks.GetLength(1); y++)
                for (int x = 0; x < SymbolicBlocks.GetLength(0); x++)
                    if (SymbolicBlocks[x, y] == blockType)
                        return new Point(x, y);

            return null;
        }

        public Vector2 BlockCoordsToPixels(Point blockCoords)
        {
            return new Vector2(blockCoords.X * BlockSize, blockCoords.Y * BlockSize);
        }

        public void RecalculateCollisionBlocks()
        {
            for (int y = 0; y < VisualBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < VisualBlocks.GetLength(0); x++)
                {
                    CollisionBlocks[x, y].TileType = AStar.TileType.Floor;

                    switch (VisualBlocks[x, y])
                    {
                        case VisualBlock.Wall:
                        case VisualBlock.UnpassableGround:
                            CollisionBlocks[x, y].TileType = AStar.TileType.Wall;
                            break;
                    }

                    switch (SymbolicBlocks[x, y])
                    {
                        case SymbolicBlock.Chest:
                        case SymbolicBlock.LockedDoor:
                            CollisionBlocks[x, y].TileType = AStar.TileType.Wall;
                            break;
                    }
                }
            }
        }
    }
}
