using Artemis;
using Artemis.System;
using Artemis.Utils;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

namespace SummerProject
{
    class Server
    {
        NetServer server;
        int playerCount = 0;


        /// <summary>
        /// Binds to socket and spawns the networking thread.
        /// </summary>
        public void Start()
        {
            var config = new NetPeerConfiguration("SummerProject");
            config.Port = Constants.NetworkPort;
            config.MaximumConnections = 10;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            server = new NetServer(config);
            server.RegisterReceivedCallback(OnMessageReceived);
            server.Start();
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
                // A DiscoveryRequest is sent to a server when a client is attempting to detect
                // running servers on a specific host or subnet. By sending a DiscoveryResponse,
                // we inform the client of our server's existance.
                case NetIncomingMessageType.DiscoveryRequest:
                    string serverName = Environment.UserDomainName + "\\" + Environment.UserName;
                    NetOutgoingMessage response = server.CreateMessage();
                    response.Write(serverName);
                    server.SendDiscoveryResponse(response, message.SenderEndPoint);
                    break;

                // A StatusChanged message to sent to the server when the status of a connection
                // between the server and a client changes. We log all such changes to the console.
                case NetIncomingMessageType.StatusChanged:
                    var status = (NetConnectionStatus)message.ReadByte();
                    string reason = message.ReadString();
                    Console.WriteLine("[SERVER] " + NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                    if (status == NetConnectionStatus.Connected && message.SenderConnection.RemoteHailMessage != null)
                        Console.WriteLine("[SERVER] Remote hail: " + message.SenderConnection.RemoteHailMessage.ReadString());
                    break;

                case NetIncomingMessageType.Data:
                    HandleDataMessage(message);
                    break;

#if DEBUG
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine("[DEBUG] [SERVER] " + text);
                    break;

                default:
                    Console.WriteLine("[DEBUG] [SERVER] Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes " + message.DeliveryMethod + "|" + message.SequenceChannel);
                    break;
#endif
            }

            server.Recycle(message);
        }


        /// <summary>
        /// Called when the client receives a data message from the server.
        /// </summary>
        private void HandleDataMessage(NetIncomingMessage message)
        {
            NetOutgoingMessage response;

            var type = (ClientMessage)message.ReadByte();
            switch (type)
            {
                case ClientMessage.RequestUniquePlayerId:
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.RequestUniquePlayerIdResponse);
                    response.Write(playerCount++);
                    server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;

                case ClientMessage.RequestWorldState:
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.RequestWorldStateResponse);
                    EntityWorld entityWorld = EntitySystem.BlackBoard.GetEntry<EntityWorld>("EntityWorld");
                    Bag<Entity> players = entityWorld.GroupManager.GetEntities("players");
                    int numberOfEntries = players.Count;
                    response.Write(numberOfEntries);
                    while ((--numberOfEntries) >= 0) {
                        int id = players[numberOfEntries].GetComponent<PlayerInfo>().PlayerId;
                        response.Write(id);
                        Point position = players[numberOfEntries].GetComponent<Transform>().Position.Round();
                        response.Write(position.X);
                        response.Write(position.Y);
                    }
                    server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;

                case ClientMessage.PlayerCreated:
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.PlayerCreated);
                    int uniqueId = message.ReadInt32();
                    response.Write(uniqueId);
                    var playerPos = new Point {
                        X = message.ReadInt32(),
                        Y = message.ReadInt32()
                    };
                    response.Write(playerPos.X);
                    response.Write(playerPos.Y);
                    server.SendToAll(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                    break;
            }
        }
    }
}
