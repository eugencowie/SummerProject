using System.IO;

namespace SummerProject
{
    static class TilemapLoader
    {
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

        public static TilemapComponent ReadMapFromFile(string file)
        {
            string[] fileLines = File.ReadAllLines(file);

            VisualBlock[,] visual = new VisualBlock[fileLines[0].Length, fileLines.Length];
            SymbolicBlock[,] symbolic = new SymbolicBlock[fileLines[0].Length, fileLines.Length];

            // Fill the map with ground by default.
            for (int j = 0; j < visual.GetLength(1); j++)
                for (int i = 0; i < visual.GetLength(0); i++)
                    visual[i, j] = VisualBlock.Ground;

            string line;
            for (int i = 0; i < fileLines.Length; ++i)
            {
                line = fileLines[i];
                for (int j = 0; j < line.Length; ++j)
                {
                    VisualBlock? vb = null;
                    SymbolicBlock? sb = null;

                    switch (line[j]) {
                        case '0': vb = VisualBlock.Wall; break;
                        case '@': vb = VisualBlock.UnpassableGround; break;
                        case '#': vb = VisualBlock.LockedGround; break;
                        case '=': vb = VisualBlock.LockedDoor; break;
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

                    if (vb.HasValue && i < visual.GetLength(0) && j < visual.GetLength(1))
                        visual[j, i] = vb.Value;
                    if (sb.HasValue && i < symbolic.GetLength(0) && j < symbolic.GetLength(1))
                        symbolic[j, i] = sb.Value;
                }
            }

            return new TilemapComponent() {
                VisualBlocks = visual,
                SymbolicBlocks = symbolic,
                BlockSize = 40
            };
        }
    }
}
