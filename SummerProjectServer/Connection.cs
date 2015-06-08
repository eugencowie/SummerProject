using Lidgren.Network;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SummerProjectServer
{
    class Connection
    {
        public string Name;
        public Vector2 Position;
        public int Timeout;  // time since last message

        public Connection(string name, Vector2 position, int timeout)
        {
            Name = name;
            Position = position;
            Timeout = timeout;
        }
    }
}
