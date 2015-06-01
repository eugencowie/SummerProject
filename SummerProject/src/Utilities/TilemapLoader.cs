using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace SummerProject
{
    static class TilemapLoader
    {
        public static Tilemap ReadMapFromFile(string file)
        {
            // Read file.
            TmxMap map = new TmxMap(file);

            // Create tile array.
            Tile[,] tiles = new Tile[map.Width, map.Height];

            // Get Tiled map layers.
            TmxLayer baseLayer = map.Layers["Base layer"];
            TmxLayer objectLayer = map.Layers["Object layer"];

            // Get Tiled tileset initial gids.
            int firstBaseGid = map.Tilesets["Base tileset"].FirstGid - 1;
            int firstObjectGid = map.Tilesets["Object tileset"].FirstGid - 1;

            for (int i = 0; i < baseLayer.Tiles.Count; i++)
            {
                TmxLayerTile baseTile = baseLayer.Tiles[i];
                TmxLayerTile objectTile = objectLayer.Tiles[i];

                int x = baseTile.X;
                int y = baseTile.Y;

                // Fill the visual blocks with none by default.
                BaseBlock baseBlock = BaseBlock.None;

                // Fill the symbolic blocks with none by default.
                ObjectBlock objectBlock = ObjectBlock.None;

                switch (baseTile.Gid - firstBaseGid) {
                    case 1: /* block */ baseBlock = BaseBlock.Wall; break;
                    case 2: /* ground */ baseBlock = BaseBlock.Ground; break;
                    case 3: /* unpassable ground */ baseBlock = BaseBlock.UnpassableGround; break;
                }

                switch (objectTile.Gid - firstObjectGid) {
                    case 1: /* player start */ objectBlock = ObjectBlock.PlayerStart; break;
                    case 2: /* chest */ objectBlock = ObjectBlock.Chest; break;
                    case 3: /* key */ objectBlock = ObjectBlock.Key; break;
                    case 4: /* door */ objectBlock = ObjectBlock.LockedDoor; break;
                }

                // Default rotation is 0.
                float baseRot = 0.0f;
                SpriteEffects baseEffect = SpriteEffects.None;
                if (baseTile.HorizontalFlip)
                    baseEffect ^= SpriteEffects.FlipHorizontally;
                if (baseTile.VerticalFlip)
                    baseEffect ^= SpriteEffects.FlipVertically;
                if (baseTile.DiagonalFlip)
                {
                    if (baseTile.HorizontalFlip && baseTile.VerticalFlip) {
                        baseRot = (float)(Math.PI / 2);
                        baseEffect ^= SpriteEffects.FlipVertically;
                    }
                    else if (baseTile.HorizontalFlip) {
                        baseRot = (float)-(Math.PI / 2);
                        baseEffect ^= SpriteEffects.FlipVertically;
                    }
                    else {
                        baseRot = (float)(Math.PI / 2);
                        baseEffect ^= SpriteEffects.FlipHorizontally;
                    }
                }

                // Default rotation is 0.
                float objectRot = 0.0f;
                SpriteEffects objectEffect = SpriteEffects.None;
                if (objectTile.HorizontalFlip)
                    objectEffect ^= SpriteEffects.FlipHorizontally;
                if (objectTile.VerticalFlip)
                    objectEffect ^= SpriteEffects.FlipVertically;
                if (objectTile.DiagonalFlip)
                {
                    if (objectTile.HorizontalFlip && baseTile.VerticalFlip) {
                        objectRot = (float)(Math.PI / 2);
                        objectEffect ^= SpriteEffects.FlipVertically;
                    }
                    else if (objectTile.HorizontalFlip) {
                        objectRot = (float)-(Math.PI / 2);
                        objectEffect ^= SpriteEffects.FlipVertically;
                    }
                    else {
                        objectRot = (float)(Math.PI / 2);
                        objectEffect ^= SpriteEffects.FlipHorizontally;
                    }
                }

                tiles[x, y].Base = baseBlock;
                tiles[x, y].Object = objectBlock;
                tiles[x, y].BaseRotation = baseRot;
                tiles[x, y].ObjectRotation = objectRot;
                tiles[x, y].BaseEffect = baseEffect;
                tiles[x, y].ObjectEffect = objectEffect;

                // Fill the collision blocks with floor by default.
                tiles[x, y].Collision = new AStar.TileInfo() { TileType = AStar.TileType.Floor };
            }

            Tilemap tilemap = new Tilemap() {
                Tiles = tiles,
                BlockSize = 40
            };

            tilemap.RecalculateCollisionBlocks();

            return tilemap;
        }
    }
}
