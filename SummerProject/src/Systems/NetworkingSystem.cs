using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class NetworkingSystem : ProcessingSystem
    {
        public static Server Server
        {
            get { return server ?? (server = new Server()); }
        }

        public static Client Client {
            get { return client ?? (client = new Client()); }
        }

        private static Server server;
        private static Client client;


        public override void ProcessSystem()
        {
        }
    }
}
