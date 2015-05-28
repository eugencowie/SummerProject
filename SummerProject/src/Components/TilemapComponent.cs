using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;

namespace SummerProject
{
    enum VisualBlock {
        None,
        Ground,
        UnpassableGround,
        LockedGround,
        LockedDoor,
        Wall,
    }

    enum SymbolicBlock {
        None,
        PlayerStart,
        General,
        Boss,
        Mob,
        MobSpawn,
        Chest,
        Trap,
        HealthPack,
        Button,
    }

    class TilemapComponent : IComponent
    {
        public VisualBlock[,] VisualBlocks;
        public SymbolicBlock[,] SymbolicBlocks;
        public AStar.TileInfo[,] CollisionBlocks;
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
    }
}
