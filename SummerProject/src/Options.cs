using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SummerProject
{
    /// <summary>
    /// Serialisable data structure containing user options which should be saved to
    /// disk.
    /// </summary>
    [Serializable]
    public struct OptionData {
        public int Width;
        public int Height;
        public bool Fullscreen;
        public bool VSync;
    }


    /// <summary>
    /// This static class contains helper functions for reading from and writing to the
    /// options file.
    /// </summary>
    static class Options
    {
        /// <summary>
        /// Static instance of the above struct containing the current user options.
        /// </summary>
        public static OptionData Instance = new OptionData {
            Width = 800,
            Height = 600,
            Fullscreen = false,
            VSync = true
        };


        /// <summary>
        /// Helper method for loading the options data from a file.
        /// </summary>
        public static void LoadOptionData(string path)
        {
            // Simply use default options if the specified file doesn't exist.
            if (!File.Exists(path)) {
                Instance.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Instance.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Instance.Fullscreen = true;
                Instance.VSync = true;
                return;
            }

            using (FileStream f = File.OpenRead(path)) {
                var xml = new XmlSerializer(Instance.GetType());
                Instance = (OptionData)xml.Deserialize(f);
            }
        }


        /// <summary>
        /// Helper method for writing the options data to a file.
        /// </summary>
        public static void WriteOptionData(string path)
        {
            // Attempt to write to a temporary file first.
            string tmpPath = path + "~";
            using (FileStream f = File.OpenWrite(tmpPath)) {
                var xml = new XmlSerializer(Instance.GetType());
                xml.Serialize(f, Instance);
            }

            // If that was successful, replace the existing options file with the
            // temporary one.
            if (File.Exists(tmpPath))
            {
                if (File.Exists(path))
                    File.Delete(path);

                File.Move(tmpPath, path);
            }
        }
    }
}
