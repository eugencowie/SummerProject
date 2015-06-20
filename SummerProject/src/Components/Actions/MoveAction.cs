using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    class MoveAction : IComponent
    {
        public Vector2 Destination;
        public float Speed = 1f;
    }
}
