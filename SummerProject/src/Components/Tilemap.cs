using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    enum BaseBlock {
        None,
        Ground,
        UnpassableGround,
        Wall,
    }

    enum ObjectBlock {
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
        public BaseBlock Base;
        public ObjectBlock Object;
        public AStar.TileInfo Collision;
        public float BaseRotation;
        public float ObjectRotation;
        public SpriteEffects BaseEffect;
        public SpriteEffects ObjectEffect;
    }

    class Tilemap : IComponent
    {
        public Tile[,] Tiles;
        public int BlockSize;

        public Point? FirstObjectBlockOfType(ObjectBlock blockType)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    if (Tiles[x, y].Object == blockType)
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

                    switch (Tiles[x, y].Base)
                    {
                        case BaseBlock.Wall:
                        case BaseBlock.UnpassableGround:
                            Tiles[x, y].Collision.TileType = AStar.TileType.Wall;
                            break;
                    }

                    switch (Tiles[x, y].Object)
                    {
                        case ObjectBlock.Chest:
                        case ObjectBlock.LockedDoor:
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
