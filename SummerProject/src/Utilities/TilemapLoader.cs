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

            // Get Tiled map layers.
            TmxLayer baseLayer = map.Layers[0];
            TmxLayer objectLayer = map.Layers[1];

            for (int i = 0; i < baseLayer.Tiles.Count; i++)
            {
                TmxLayerTile baseTile = baseLayer.Tiles[i];
                TmxLayerTile objectTile = objectLayer.Tiles[i];

                int x = baseTile.X;
                int y = baseTile.Y;

                // Fill the visual blocks with none by default.
                VisualBlock vb = VisualBlock.None;

                // Fill the symbolic blocks with none by default.
                SymbolicBlock sb = SymbolicBlock.None;

                // Fill the collision blocks with floor by default.
                AStar.TileInfo cbinfo = new AStar.TileInfo();
                AStar.TileType cb = AStar.TileType.Floor;

                switch (baseTile.Gid) {
                    case 1: vb = VisualBlock.Wall; cb = AStar.TileType.Wall; break;
                    case 2: vb = VisualBlock.Ground; break;
                    case 4: vb = VisualBlock.UnpassableGround; cb = AStar.TileType.Wall; break;
                }

                switch (objectTile.Gid) {
                    case 3: sb = SymbolicBlock.PlayerStart; break;
                }

                visual[x, y] = vb;
                symbolic[x, y] = sb;

                cbinfo.TileType = cb;
                collision[x, y] = cbinfo;
            }

            return new TilemapComponent() {
                VisualBlocks = visual,
                SymbolicBlocks = symbolic,
                CollisionBlocks = collision,
                BlockSize = 40
            };
        }
    }
}
