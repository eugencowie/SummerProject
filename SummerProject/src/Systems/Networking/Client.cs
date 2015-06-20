using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace SummerProject
{
    /// <summary>
    /// TODO
    /// </summary>
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class Client : ProcessingSystem
    {
        public static bool Active = false;
        public static Client Instance;

        NetClient client;

        public Client()
        {
            if (Instance != null)
                throw new InvalidOperationException();

            Instance = this;
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public override void ProcessSystem()
        {
            // Check whether the server should be active or not.
            if (!Active)
            {
                if (client == null)
                    return;

                if (client.Status != NetPeerStatus.NotRunning)
                    client.Shutdown("client shutdown requested");

                client = null;
                return;
            }

            // Make sure that the client has been initialised.
            if (client == null)
            {
                NetPeerConfiguration config = new NetPeerConfiguration("SummerProject");
                config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

                client = new NetClient(config);
                client.RegisterReceivedCallback(OnMessageReceived);
            }

            // Make sure that the client is connected.
            if (client.Status == NetPeerStatus.NotRunning)
            {
                client.Start();

                // If host.txt exists, attempt to connect to the host specified...
                if (File.Exists("host.txt")) {
                    string host = "127.0.0.1";
                    using (StreamReader f = File.OpenText("host.txt"))
                        host = f.ReadLine();
                    client.DiscoverKnownPeer(host, 14242);
                }
                // ... otherwise attempt to discover hosts on the local network.
                else
                    client.DiscoverLocalPeers(14242);
            }
        }

        /// <summary>
        /// Called when a message is received.
        /// </summary>
        private void OnMessageReceived(object peer)
        {
            NetIncomingMessage message = ((NetPeer)peer).ReadMessage();

            if (message == null)
                return;

            switch (message.MessageType)
            {
                // A DiscoveryResponse is sent to the client by a server after the client has sent
                // a DiscoveryRequest to that server. A DiscoveryResponse indicates that the server
                // is running and able to be connected to.
                case NetIncomingMessageType.DiscoveryResponse:
                    client.Connect(message.SenderEndPoint.Address.ToString(), message.SenderEndPoint.Port);
                    break;

                // A StatusChanged message to sent to the client when the status of the connection
                // between the client and server changes. We log all such changes to the console.
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                    string reason = message.ReadString();
                    Console.WriteLine("[CLIENT] " + status + ": " + reason);
                    break;

                case NetIncomingMessageType.Data:
                    HandleDataMessage(message);
                    break;

#if DEBUG
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine("[DEBUG] [CLIENT] " + text);
                    break;

                default:
                    Console.WriteLine("[DEBUG] [CLIENT] Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes");
                    break;
#endif
            }

            client.Recycle(message);
        }

        /// <summary>
        /// Called when the client receives a data message from the server.
        /// </summary>
        private void HandleDataMessage(NetIncomingMessage message)
        {
            NetOutgoingMessage response;

            ServerMessage type = (ServerMessage)message.ReadByte();
            switch (type)
            {
                // TODO
                case ServerMessage.CreateRemotePlayer:
                    entityWorld.TagManager.GetEntity("player").GetComponent<PlayerInfo>().LocalPlayer = false;
                    break;

                // TODO
                case ServerMessage.MoveRemotePlayer:
                    int x = message.ReadInt32();
                    int y = message.ReadInt32();
                    Entity entity = entityWorld.TagManager.GetEntity("player");
                    if (!entity.GetComponent<PlayerInfo>().LocalPlayer) {
                        // If the player is already moving, stop and go to the new destination instead.
                        if (entity.HasComponent<MoveAction>())
                            entity.RemoveComponent<MoveAction>();
                        // The move action tells the movement system to move the player to the specified destination.
                        entity.AddComponent(new MoveAction {
                            Destination = new Vector2(x, y),
                            Speed = 5.75f // make player slightly faster to compensate for lag - TODO: implement something more robust if necessary
                        });
                    }
                    break;
            }
        }

        public void SendIsReadyMessage()
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write((byte)ClientMessage.IsReady);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMoveMessage(int destX, int destY)
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write((byte)ClientMessage.PlayerHasMoved);
            om.Write(destX);
            om.Write(destY);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
