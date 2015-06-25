using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace SummerProject
{
    static class TilemapLoader
    {
        /// <summary>
        /// Helper function to create a Tilemap component from a Tiled TMX map file.
        /// </summary>
        public static Tilemap ReadMapFromFile(string file, EntityWorld entityManager)
        {
            // Read file.
            var map = new TmxMap(file);

            // Create tile array.
            var tiles = new Tile[map.Width, map.Height];

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
                var baseBlock = BaseBlock.None;

                // Fill the symbolic blocks with none by default.
                var objectBlock = ObjectBlock.None;

                ContentManager content = EntitySystem.BlackBoard.GetEntry<Game>("Game").Content;

                string baseTexture = "";
                switch (baseTile.Gid - firstBaseGid) {
                    case 1: /* block */
                        baseBlock = BaseBlock.Wall;
                        baseTexture = "textures/block";
                        break;
                    case 2: /* ground */
                        baseBlock = BaseBlock.Ground;
                        baseTexture = "textures/ground";
                        break;
                    case 3: /* unpassable ground */
                        baseBlock = BaseBlock.UnpassableGround;
                        baseTexture = "textures/unpassable_ground";
                        break;
                }

                string objectTexture = "";
                switch (objectTile.Gid - firstObjectGid) {
                    case 1: /* player start */
                        objectBlock = ObjectBlock.PlayerStart;
                        break;
                    case 2: /* chest */
                        objectBlock = ObjectBlock.Chest;
                        objectTexture = "textures/objects/chest";
                        break;
                    case 3: /* key */
                        objectBlock = ObjectBlock.Key;
                        objectTexture = "textures/objects/key";
                        break;
                    case 4: /* door */
                        objectBlock = ObjectBlock.LockedDoor;
                        objectTexture = "textures/objects/locked_door";
                        break;
                    case 5: /* mob */
                        objectBlock = ObjectBlock.Mob;
                        break;
                }

                // Default rotation is 0.
                float baseRot = 0f;
                var baseEffect = SpriteEffects.None;
                if (baseTile.HorizontalFlip)
                    baseEffect ^= SpriteEffects.FlipHorizontally;
                if (baseTile.VerticalFlip)
                    baseEffect ^= SpriteEffects.FlipVertically;
                if (baseTile.DiagonalFlip)
                {
                    if (baseTile.HorizontalFlip && baseTile.VerticalFlip) {
                        baseRot = (float)(Math.PI / 2f);
                        baseEffect ^= SpriteEffects.FlipVertically;
                    }
                    else if (baseTile.HorizontalFlip) {
                        baseRot = -(float)(Math.PI / 2f);
                        baseEffect ^= SpriteEffects.FlipVertically;
                    }
                    else {
                        baseRot = (float)(Math.PI / 2f);
                        baseEffect ^= SpriteEffects.FlipHorizontally;
                    }
                }

                Entity baseEntity = null;
                if (!string.IsNullOrEmpty(baseTexture))
                {
                    var position = new Vector2(x, y);
                    baseEntity = entityManager.CreateEntity("background")
                        .AddBackgroundTileComponents(content, position, baseRot, baseTexture, baseEffect);
                }

                // Default rotation is 0.
                float objectRot = 0f;
                var objectEffect = SpriteEffects.None;
                if (objectTile.HorizontalFlip)
                    objectEffect ^= SpriteEffects.FlipHorizontally;
                if (objectTile.VerticalFlip)
                    objectEffect ^= SpriteEffects.FlipVertically;
                if (objectTile.DiagonalFlip)
                {
                    if (objectTile.HorizontalFlip && baseTile.VerticalFlip) {
                        objectRot = (float)(Math.PI / 2f);
                        objectEffect ^= SpriteEffects.FlipVertically;
                    }
                    else if (objectTile.HorizontalFlip) {
                        objectRot = (float)-(Math.PI / 2f);
                        objectEffect ^= SpriteEffects.FlipVertically;
                    }
                    else {
                        objectRot = (float)(Math.PI / 2f);
                        objectEffect ^= SpriteEffects.FlipHorizontally;
                    }
                }

                Entity objectEntity = null;
                if (!string.IsNullOrEmpty(objectTexture))
                {
                    var position = new Vector2(x, y);
                    objectEntity = entityManager.CreateEntity("objects")
                        .AddObjectTileComponents(content, position, objectRot, objectTexture, objectEffect);
                }

                if (baseEntity != null)
                    tiles[x, y].BaseEntity = baseEntity;

                if (objectEntity != null)
                    tiles[x, y].ObjectEntity = objectEntity;

                tiles[x, y].Base = baseBlock;
                tiles[x, y].Object = objectBlock;
                tiles[x, y].BaseRotation = baseRot;
                tiles[x, y].ObjectRotation = objectRot;
                tiles[x, y].BaseEffect = baseEffect;
                tiles[x, y].ObjectEffect = objectEffect;

                // Fill the collision blocks with floor by default.
                tiles[x, y].Collision = new AStar.TileInfo { TileType = AStar.TileType.Floor };
            }

            var tilemap = new Tilemap {
                Tiles = tiles,
            };

            tilemap.RecalculateCollisionBlocks();

            return tilemap;
        }
    }
}
