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

    struct Tile {
        public VisualBlock Visual;
        public SymbolicBlock Symbolic;
        public AStar.TileInfo Collision;
        public int Rotation;
    }

    class Tilemap : IComponent
    {
        public Tile[,] Tiles;
        public int BlockSize;

        public Point? FirstSymbolicBlockOfType(SymbolicBlock blockType)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    if (Tiles[x, y].Symbolic == blockType)
                        return new Point(x, y);

            return null;
        }

        public Vector2 BlockCoordsToPixels(Point blockCoords)
        {
            return new Vector2(blockCoords.X * BlockSize, blockCoords.Y * BlockSize);
        }

        public void RecalculateCollisionBlocks()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y].Collision.TileType = AStar.TileType.Floor;

                    switch (Tiles[x, y].Visual)
                    {
                        case VisualBlock.Wall:
                        case VisualBlock.UnpassableGround:
                            Tiles[x, y].Collision.TileType = AStar.TileType.Wall;
                            break;
                    }

                    switch (Tiles[x, y].Symbolic)
                    {
                        case SymbolicBlock.Chest:
                        case SymbolicBlock.LockedDoor:
                            Tiles[x, y].Collision.TileType = AStar.TileType.Wall;
                            break;
                    }
                }
            }
        }

        public AStar.TileInfo[,] GetCollisionInfo()
        {
            AStar.TileInfo[,] collision = new AStar.TileInfo[Tiles.GetLength(0), Tiles.GetLength(1)];

            for (int y = 0; y < Tiles.GetLength(1); y++)
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    collision[x, y] = Tiles[x, y].Collision;

            return collision;
        }
    }
}
