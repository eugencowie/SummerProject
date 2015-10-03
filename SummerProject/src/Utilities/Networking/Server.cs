using Artemis;
using Artemis.System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace SummerProject
{
    class Server
    {
        NetServer server;
        int playerCount;

        Dictionary<NetConnection, int> playerIds = new Dictionary<NetConnection, int>();


        /// <summary>
        /// Binds to socket and spawns the networking thread.
        /// </summary>
        /// <param name="maximumAttempts">
        /// If set to a value greater than one, will attempt to use alternative ports if
        /// the specified port is already in use. If set to one, will only attempt to use
        /// the specified port.
        /// </param>
        /// <returns>The port that was successfully used to start the server.</returns>
        public int Start(int port, int maximumAttempts=1)
        {
            int maxPort = port + maximumAttempts;
            while (port < maxPort)
            {
                var config = new NetPeerConfiguration("SummerProject") {
                    Port = port,
                    MaximumConnections = 10,
                    PingInterval = 1f,
                    ConnectionTimeout = 3f
                };
                config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

                server = new NetServer(config);
                server.RegisterReceivedCallback(OnMessageReceived);

                try
                {
                    server.Start();
                    break;
                }
                catch (SocketException e)
                {
                    if (port >= maxPort)
                        throw e;
                    else
                        port++;
                }
            }

            return port;
        }


        /// <summary>
        /// Disconnects all active connections and closes the socket.
        /// </summary>
        public void Stop()
        {
            if (server != null)
                server.Shutdown("user disconnect");
        }


        /// <summary>
        /// Called when a message is received.
        /// </summary>
        private void OnMessageReceived(object peer)
        {
            NetIncomingMessage message = ((NetPeer)peer).ReadMessage();
            
            if (message == null) 
                return;

            NetOutgoingMessage response;
            switch (message.MessageType)
            {
                // A DiscoveryRequest is sent to a server when a client is attempting to detect
                // running servers on a specific host or subnet. By sending a DiscoveryResponse,
                // we inform the client of our server's existance.
                case NetIncomingMessageType.DiscoveryRequest:
                    string serverName = Environment.UserDomainName + "\\" + Environment.UserName;
                    response = server.CreateMessage();
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
                    if (status == NetConnectionStatus.Disconnected && playerIds.ContainsKey(message.SenderConnection)) {
                        response = server.CreateMessage();
                        response.Write((byte)ServerMessage.PlayerRemoved);
                        response.Write(playerIds[message.SenderConnection]);
                        server.SendToAll(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        playerIds.Remove(message.SenderConnection);
                    }
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
                case ClientMessage.RequestUniquePlayerId: {
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.RequestUniquePlayerIdResponse);
                    response.Write(playerCount++);
                    server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;
                }

                case ClientMessage.RequestWorldState: {
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.RequestWorldStateResponse);
                    EntityWorld entityWorld = EntitySystem.BlackBoard.GetEntry<EntityWorld>("EntityWorld");
                    IEnumerable<Entity> enumerable = entityWorld.GroupManager.GetEntities("players").Where(entity => entity.IsActive);
                    IEnumerable<Entity> players = enumerable as Entity[] ?? enumerable.ToArray();
                    int numberOfEntries = players.Count();
                    response.Write(numberOfEntries);
                    while ((--numberOfEntries) >= 0) {
                        int id = players.ElementAt(numberOfEntries).GetComponent<PlayerInfo>().PlayerId;
                        int playerType = players.ElementAt(numberOfEntries).GetComponent<PlayerInfo>().PlayerType;
                        Point position = players.ElementAt(numberOfEntries).GetComponent<Transform>().Position.Round();
                        response.Write(id);
                        response.Write(playerType);
                        response.Write(position.X);
                        response.Write(position.Y);
                    }
                    server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;
                }

                case ClientMessage.PlayerCreated: {
                    int uniqueId = message.ReadInt32();
                    int playerType = message.ReadInt32();
                    var playerPos = new Point {
                        X = message.ReadInt32(),
                        Y = message.ReadInt32()
                    };
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.PlayerCreated);
                    response.Write(uniqueId);
                    response.Write(playerType);
                    response.Write(playerPos.X);
                    response.Write(playerPos.Y);
                    server.SendToAll(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                    playerIds.Add(message.SenderConnection, uniqueId);
                    break;
                }

                case ClientMessage.PlayerMoved: {
                    int uniqueId = message.ReadInt32();
                    var dest = new Point {
                        X = message.ReadInt32(),
                        Y = message.ReadInt32()
                    };
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.PlayerMoved);
                    response.Write(uniqueId);
                    response.Write(dest.X);
                    response.Write(dest.Y);
                    server.SendToAll(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                    break;
                }
            }
        }
    }
}
