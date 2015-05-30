using TiledSharp;

namespace SummerProject
{
    static class TilemapLoader
    {
        public static TilemapComponent ReadMapFromFile(string file)
        {
            // Read file.
            TmxMap map = new TmxMap(file);

            // Create block arrays.
            VisualBlock[,] visual = new VisualBlock[map.Width, map.Height];
            SymbolicBlock[,] symbolic = new SymbolicBlock[map.Width, map.Height];
            AStar.TileInfo[,] collision = new AStar.TileInfo[map.Width, map.Height];
            int[,] rotation = new int[map.Width, map.Height];

            // Get Tiled map layers.
            TmxLayer baseLayer = map.Layers[0];

            for (int i = 0; i < baseLayer.Tiles.Count; i++)
            {
                TmxLayerTile tile = baseLayer.Tiles[i];

                int x = tile.X;
                int y = tile.Y;

                // Fill the visual blocks with none by default.
                VisualBlock vb = VisualBlock.None;

                // Fill the symbolic blocks with none by default.
                SymbolicBlock sb = SymbolicBlock.None;

                // Default rotation is 0.
                int rot = 0;

                switch (tile.Gid) {
                    case 1: /* wall */ vb = VisualBlock.Wall; break;
                    case 2: /* ground */ vb = VisualBlock.Ground; break;
                    case 3: /* player start */ vb = VisualBlock.Ground; sb = SymbolicBlock.PlayerStart; break;
                    case 4: /* unpassable ground */ vb = VisualBlock.UnpassableGround; break;
                    case 5: /* chest */ vb = VisualBlock.Ground; sb = SymbolicBlock.Chest; break;
                    case 6: /* door */ vb = VisualBlock.Ground; sb = SymbolicBlock.LockedDoor; break;
                    case 7: /* vertical door */ vb = VisualBlock.Ground; sb = SymbolicBlock.LockedDoor; rot = 90; break;
                    case 8: /* key */ vb = VisualBlock.Ground; sb = SymbolicBlock.Key; break;
                }

                visual[x, y] = vb;
                symbolic[x, y] = sb;
                rotation[x, y] = rot;

                // Fill the collision blocks with floor by default.
                collision[x, y] = new AStar.TileInfo() { TileType = AStar.TileType.Floor };
            }

            TilemapComponent tilemap = new TilemapComponent() {
                VisualBlocks = visual,
                SymbolicBlocks = symbolic,
                CollisionBlocks = collision,
                Rotations = rotation,
                BlockSize = 40
            };

            tilemap.RecalculateCollisionBlocks();

            return tilemap;
        }
    }
}
