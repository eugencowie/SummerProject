using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class NetworkingSystem : ProcessingSystem
    {
        public static Server Server = new Server();
        public static Client Client = new Client();

        public override void ProcessSystem()
        {
        }
    }
}
