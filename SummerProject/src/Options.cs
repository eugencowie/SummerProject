﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SummerProject
{
    [Serializable]
    public struct OptionData {
        public int Width;
        public int Height;
        public bool Fullscreen;
        public bool VSync;
    }


    /// <summary>
    /// This class contains helper functions for reading and writing to an options file.
    /// </summary>
    static class Options
    {
        public static OptionData Instance = new OptionData {
            Width = 800,
            Height = 600,
            Fullscreen = false,
            VSync = true
        };


        public static void LoadOptionData(string path)
        {
            if (!File.Exists(path))
            {
                // Use default options.
                Instance.Fullscreen = true;
                Instance.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Instance.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                return;
            }

            using (FileStream f = File.OpenRead(path)) {
                var xml = new XmlSerializer(Instance.GetType());
                Instance = (OptionData)xml.Deserialize(f);
            }
        }


        public static void WriteOptionData(string path)
        {
            string tmpPath = path + "~";
            using (FileStream f = File.OpenWrite(tmpPath)) {
                var xml = new XmlSerializer(Instance.GetType());
                xml.Serialize(f, Instance);
            }

            if (File.Exists(tmpPath))
            {
                if (File.Exists(path))
                    File.Delete(path);

                File.Move(tmpPath, path);
            }
        }
    }
}
