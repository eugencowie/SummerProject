using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SummerProject
{
    enum BaseBlock {
        None,
        Ground,
        UnpassableGround,
        Wall
    }

    enum ObjectBlock {
        None,
        LockedDoor,
        PlayerStart,
        //General,
        //Boss,
        Mob,
        //MobSpawn,
        Chest,
        //Trap,
        //HealthPack,
        //Button,
        Key
    }

    struct Tile
    {
        public Entity BaseEntity;
        public Entity ObjectEntity;

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

        /// <summary>
        /// Find the first object block in the tilemap which matches the specified object
        /// block type. Can be useful to find the player start, for example.
        /// </summary>
        public Point? FirstObjectBlockOfType(ObjectBlock blockType)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    if (Tiles[x, y].Object == blockType)
                        return new Point(x, y);

            return null;
        }

        /// <summary>
        /// Find all object blocks in the tilemap which match the specified object block
        /// type. Can be useful for finding mob spawns, for example.
        /// </summary>
        public List<Point> AllObjectBlocksOfType(ObjectBlock blockType)
        {
            var list = new List<Point>();

            for (int y = 0; y < Tiles.GetLength(1); y++)
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    if (Tiles[x, y].Object == blockType)
                        list.Add(new Point(x, y));

            return list;
        }

        /// <summary>
        /// Convert block coords to pixels.
        /// </summary>
        public static Vector2 BlockCoordsToPixels(Point blockCoords)
        {
            return new Vector2 {
                X = blockCoords.X * Constants.UnitSize,
                Y = blockCoords.Y * Constants.UnitSize
            };
        }

        /// <summary>
        /// Convert pixels to block coords.
        /// </summary>
        public static Point PixelsToBlockCoords(Vector2 pixels)
        {
            return new Point {
                X = (int)Math.Round(pixels.X / Constants.UnitSize),
                Y = (int)Math.Round(pixels.Y / Constants.UnitSize)
            };
        }

        /// <summary>
        /// Recalculate the collision block information. Usually needed after removing an
        /// obstacle such as a door, for example.
        /// </summary>
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

        /// <summary>
        /// Generate an AStar-compatible array of collision block information.
        /// </summary>
        public AStar.TileInfo[,] GetCollisionInfo()
        {
            var collision = new AStar.TileInfo[Tiles.GetLength(0), Tiles.GetLength(1)];

            for (int y = 0; y < Tiles.GetLength(1); y++)
                for (int x = 0; x < Tiles.GetLength(0); x++)
                    collision[x, y] = Tiles[x, y].Collision;

            return collision;
        }
    }
}
