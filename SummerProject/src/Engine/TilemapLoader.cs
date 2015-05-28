using System.IO;

namespace SummerProject
{
    static class TilemapLoader
    {
        public static int[,] DefaultMap = {
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
        };

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

        public static int[,] ReadMapFromFile(string file)
        {
            string[] fileLines = File.ReadAllLines(file);
            int[,] map = new int[fileLines[0].Length, fileLines.Length];

            string line;
            for (int i = 0; i < fileLines.Length; ++i)
            {
                line = fileLines[i];
                for (int j = 0; j < line.Length; ++j)
                {
                    int? c = null;
                    switch (line[j]) {
                        case '-': c = 0; break;
                        case '0': c = 1; break;
                        default: c = null; break;
                    }
                    if (c.HasValue && i < map.GetLength(0) && j < map.GetLength(1))
                        map[i, j] = c.Value;
                }
            }

            return map;
        }
    }
}
