using Artemis.Interface;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    /// <summary>
    /// The order in which sprites are rendered depends on the order of the elements
    /// in this enum. Changing the order of the elements in this enum will directly
    /// affect the order in which sprite are rendered.
    /// </summary>
    enum LayerDepth {
        Foreground,
        Player,
        Object,
        Background
    }

    class Sprite : IComponent
    {
        public Texture2D Texture = null;
        public SpriteEffects Effects = SpriteEffects.None;
        public LayerDepth LayerDepth = LayerDepth.Foreground;
    }
}
