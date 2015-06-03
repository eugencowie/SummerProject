using Artemis.Interface;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    class Sprite : IComponent
    {
        public Texture2D Texture = null;
        public SpriteEffects Effects = SpriteEffects.None;
        public float LayerDepth = 1.0f;
    }
}
