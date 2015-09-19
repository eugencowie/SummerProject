using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SummerProject
{
    /// <summary>
    /// This class manages reading from and writing to the options file.
    /// </summary>
    public class Options
    {
        public static Options Instance = new Options();

        public int Width;
        public int Height;
        public bool Fullscreen;
        public bool VSync;


        /// <summary>
        /// Resets the options to their default values.
        /// </summary>
        private void ResetToDefaults()
        {
            // Get the maximum supported display resolution.
            DisplayMode maxMode = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                .OrderByDescending(m => m.Width * m.Height)
                .First();

            // Set options to their default values.
            Width = maxMode.Width;
            Height = maxMode.Height;
            Fullscreen = true;
            VSync = true;
        }


        #region Static methods


        /// <summary>
        /// Static method for loading the options data from a file.
        /// </summary>
        public static void LoadOptionsFromFile(string path)
        {
            // Simply use default options if the specified file doesn't exist.
            if (!File.Exists(path)) {
                Instance.ResetToDefaults();
                return;
            }

            using (FileStream f = File.OpenRead(path)) {
                var xml = new XmlSerializer(typeof(Options));
                Instance = (Options)xml.Deserialize(f);
            }
        }


        /// <summary>
        /// Static method for writing the options data to a file.
        /// </summary>
        public static void WriteOptionsToFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            using (FileStream f = File.OpenWrite(path)) {
                var xml = new XmlSerializer(typeof(Options));
                xml.Serialize(f, Instance);
            }
        }


        #endregion
    }
}
