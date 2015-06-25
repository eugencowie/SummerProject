using Microsoft.Xna.Framework;
using System;

namespace SummerProject
{
    /// <summary>
    /// This static class contains global static constants which are used in various
    /// places throughout the application.
    /// </summary>
    static class Constants
    {
        /// <summary>
        /// The size of each unit of measurement in the game, in pixels.
        /// </summary>
        public const int UnitSize = 40;


        /// <summary>
        /// The file path of the options file.
        /// </summary>
        public const string OptionsFile = "options.xml";
    }


    /// <summary>
    /// This static class contains generic extension methods.
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Vector2 extension method to convert floating point unit coords to
        /// integer block coords by rounding the values and storing them in a
        /// new Point.
        /// </summary>
        public static Point Round(this Vector2 units)
        {
            return new Point {
                X = (int)Math.Round(units.X),
                Y = (int)Math.Round(units.Y)
            };
        }
    }
}
