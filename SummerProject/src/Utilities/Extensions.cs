using Microsoft.Xna.Framework;
using System;

namespace SummerProject
{
    /// <summary>
    /// This static class contains generic extension methods.
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Vector2 extension method to convert floating point unit coords to
        /// integer block coords by rounding the values to the nearest whole
        /// number and storing them in a Point.
        /// </summary>
        public static Point Round(this Vector2 units)
        {
            return new Point {
                X = (int)Math.Round(units.X),
                Y = (int)Math.Round(units.Y)
            };
        }


        /// <summary>
        /// Point extension method to convert integer block coords to floating point
        /// unit coords. Since this is just a simple int to float conversion, no
        /// information is lost (unlike the opposite Vector2.Round method above).
        /// </summary>
        public static Vector2 ToVector2(this Point block)
        {
            return new Vector2(block.X, block.Y);
        }
    }
}
