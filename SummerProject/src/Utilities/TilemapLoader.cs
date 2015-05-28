using System.IO;

namespace SummerProject
{
    static class TilemapLoader
    {
        public static TilemapComponent ReadMapFromFile(string file)
        {
            // Read file.
            string[] fileLines = File.ReadAllLines(file);

            // Get tilemap height.
            int tilemapHeight = 0;
            for (int i = 0; i < fileLines.Length; i++)
                if (fileLines[i].Length == 0 && tilemapHeight == 0)
                    tilemapHeight = i;

            // Get tilemap width.
            int tilemapWidth = 0;
            for (int i = 0; i < tilemapHeight; i++)
                if (fileLines[i].Length > tilemapWidth)
                    tilemapWidth = fileLines[i].Length;

            // Create block arrays.
            VisualBlock[,] visual = new VisualBlock[tilemapWidth, tilemapHeight];
            SymbolicBlock[,] symbolic = new SymbolicBlock[tilemapWidth, tilemapHeight];
            AStar.TileInfo[,] collision = new AStar.TileInfo[tilemapWidth, tilemapHeight];

            string line;
            for (int y = 0; y < tilemapHeight; y++)
            {
                line = fileLines[y];
                for (int x = 0; x < line.Length; ++x)
                {
                    // Fill the visual blocks with ground by default.
                    VisualBlock vb = VisualBlock.Ground;

                    // Fill the symbolic blocks with none by default.
                    SymbolicBlock sb = SymbolicBlock.None;

                    // Fill the collision blocks with floor by default.
                    AStar.TileInfo cbinfo = new AStar.TileInfo();
                    AStar.TileType cb = AStar.TileType.Floor;

                    switch (line[x]) {
                        case '0': vb = VisualBlock.Wall; cb = AStar.TileType.Wall; break;
                        case '@': vb = VisualBlock.UnpassableGround; cb = AStar.TileType.Wall; break;
                        case '#': vb = VisualBlock.LockedGround; cb = AStar.TileType.Wall; break;
                        case '=': vb = VisualBlock.LockedDoor; cb = AStar.TileType.Wall; break;
                        case 'S': sb = SymbolicBlock.PlayerStart; break;
                        case 'M': sb = SymbolicBlock.Mob; break;
                        case 'T': sb = SymbolicBlock.Trap; break;
                        case 'C': sb = SymbolicBlock.Chest; break;
                        case 'G': sb = SymbolicBlock.General; break;
                        case 'B': sb = SymbolicBlock.Button; break;
                        case 'H': sb = SymbolicBlock.HealthPack; break;
                        case 'Q': sb = SymbolicBlock.MobSpawn; break;
                        case '?': sb = SymbolicBlock.Boss; break;
                    }

                    visual[x, y] = vb;
                    symbolic[x, y] = sb;

                    cbinfo.TileType = cb;
                    collision[x, y] = cbinfo;
                }
            }

            return new TilemapComponent() {
                VisualBlocks = visual,
                SymbolicBlocks = symbolic,
                CollisionBlocks = collision,
                BlockSize = 40
            };
        }

        /*
        public static void WriteMapToFile(string file, int[,] map)
        {
            StreamWriter sw = new StreamWriter(file);

            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    char? c = null;
                    switch (map[x, y]) {
                        case 0: c = '-'; break;
                        case 1: c = '0'; break;
                    }
                    if (c.HasValue)
                        sw.Write(c.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
        }
        */
    }
}
