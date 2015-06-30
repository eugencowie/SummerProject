using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    class Pathfinder : IComponent
    {
        public Vector2 Destination;
        public float Speed = 1f;

        public AStar.AStar CurrentPath = null;
        public Vector2 CurrentDestination = Vector2.Zero;
        public int CurrentIndex = 0;
    }
}
