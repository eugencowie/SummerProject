using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    class Transform : IComponent
    {
        public Vector2 Position = Vector2.Zero;
        public Vector2 Size = Vector2.One;
        public float Rotation = 0f;
    }
}
