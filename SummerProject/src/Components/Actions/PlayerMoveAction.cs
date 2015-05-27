using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    class PlayerMoveAction : IComponent
    {
        public Vector2 Destination;
        public float Speed = 1.0f;
    }
}
