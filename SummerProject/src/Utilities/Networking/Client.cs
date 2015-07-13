using Artemis;
using Artemis.System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Linq;
using System.Net;

namespace SummerProject
{
    class Client
    {
        public delegate void LostConnectionToServerDelegate();
        private LostConnectionToServerDelegate lostConnectionToServer;

        public delegate void DiscoveryResponseDelegate(string name, IPEndPoint endpoint);
        private DiscoveryResponseDelegate discoveryResponse;

        public delegate void ConnectedToHostDelegate();
        private ConnectedToHostDelegate connectedToHost;

        public delegate void RequestUniquePlayerIdDelegate(int id);
        private RequestUniquePlayerIdDelegate requestUniquePlayerId;

        public delegate void RequestWorldStateDelegate(int id, Vector2 position);
        private RequestWorldStateDelegate requestWorldState;
        
        NetClient client;


        /// <summary>
        /// Binds to socket and spawns the networking thread.
        /// </summary>
        public void Start()
        {
            var config = new NetPeerConfiguration("SummerProject");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.RegisterReceivedCallback(OnMessageReceived);
            client.Start();
        }


        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        public void Stop()
        {
            if (client != null)
                client.Disconnect("user disconnect");
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
                    string serverName = message.ReadString();
                    if (discoveryResponse != null)
                        discoveryResponse(serverName, message.SenderEndPoint);
                    break;

                // A StatusChanged message to sent to the client when the status of the connection
                // between the client and server changes.
                case NetIncomingMessageType.StatusChanged:
                    var status = (NetConnectionStatus)message.ReadByte();
                    string reason = message.ReadString();
                    Console.WriteLine("[CLIENT] " + status + ": " + reason);
                    if (status == NetConnectionStatus.Connected && connectedToHost != null) {
                        discoveryResponse = null;
                        connectedToHost();
                        connectedToHost = null;
                    }
                    if (status == NetConnectionStatus.Disconnected && client.ServerConnection == null) {
                        if (lostConnectionToServer != null)
                            lostConnectionToServer();
                    }
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

            var type = (ServerMessage)message.ReadByte();
            switch (type)
            {
                case ServerMessage.RequestUniquePlayerIdResponse: {
                    int uniqueId = message.ReadInt32();
                    requestUniquePlayerId(uniqueId);
                    requestUniquePlayerId = null;
                    break;
                }

                case ServerMessage.RequestWorldStateResponse: {
                    int numberOfEntries = message.ReadInt32();
                    while ((--numberOfEntries) >= 0)
                    {
                        int id = message.ReadInt32();
                        var position = new Vector2 {
                            X = message.ReadInt32(),
                            Y = message.ReadInt32()
                        };
                        requestWorldState(id, position);
                    }
                    requestWorldState = null;
                    break;
                }

                case ServerMessage.PlayerCreated: {
                    int playerId = message.ReadInt32();
                    var playerPos = new Vector2 {
                        X = message.ReadInt32(),
                        Y = message.ReadInt32()
                    };
                    EntityWorld entityWorld = EntitySystem.BlackBoard.GetEntry<EntityWorld>("EntityWorld");
                    ContentManager content = EntitySystem.BlackBoard.GetEntry<ContentManager>("Content");
                    if (playerId != entityWorld.TagManager.GetEntity("player1").GetComponent<PlayerInfo>().PlayerId) {
                        entityWorld.CreateEntity(group: "players")
                            .AddPlayerComponents(content, playerId, playerPos, false);
                    }
                    break;
                }

                case ServerMessage.PlayerRemoved: {
                    int pid = message.ReadInt32();
                    EntityWorld world = EntitySystem.BlackBoard.GetEntry<EntityWorld>("EntityWorld");
                    foreach (Entity entity in world.GroupManager.GetEntities("players")
                        .Where(entity => entity.GetComponent<PlayerInfo>().PlayerId == pid))
                    {
                        world.EntityManager.Remove(entity);
                    }
                    break;
                }

                case ServerMessage.PlayerMoved: {
                    int playerId = message.ReadInt32();
                    var playerPos = new Vector2 {
                        X = message.ReadInt32(),
                        Y = message.ReadInt32()
                    };
                    EntityWorld entityWorld = EntitySystem.BlackBoard.GetEntry<EntityWorld>("EntityWorld");
                    if (playerId != entityWorld.TagManager.GetEntity("player1").GetComponent<PlayerInfo>().PlayerId)
                    {
                        foreach (Entity entity in entityWorld.GroupManager.GetEntities("players")
                            .Where(entity => entity.GetComponent<PlayerInfo>().PlayerId == playerId))
                        {
                            entity.GetComponent<Pathfinder>().Destination = playerPos;
                            entity.GetComponent<Pathfinder>().Speed = 5.75f;
                        }
                    }
                    break;
                }
            }
        }


        public void OnLostConnectionToServer(LostConnectionToServerDelegate d)
        {
            if (client == null) throw new InvalidOperationException();

            lostConnectionToServer = d;
        }


        public void DiscoverLocalPeers(int port, DiscoveryResponseDelegate d)
        {
            if (client == null) throw new InvalidOperationException();

            discoveryResponse = d;
            client.DiscoverLocalPeers(port);
        }


        public void DiscoverRemotePeers(string host, int port, DiscoveryResponseDelegate d)
        {
            if (client == null) throw new InvalidOperationException();

            discoveryResponse = d;
            client.DiscoverKnownPeer(host, port);
        }


        public void Connect(string host, int port, ConnectedToHostDelegate d)
        {
            if (client == null) throw new InvalidOperationException();

            connectedToHost = d;
            client.Connect(host, port);
        }


        public void RequestUniquePlayerId(RequestUniquePlayerIdDelegate d)
        {
            if (client == null) throw new InvalidOperationException();

            requestUniquePlayerId = d;

            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)ClientMessage.RequestUniquePlayerId);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }


        public void RequestWorldState(RequestWorldStateDelegate d)
        {
            if (client == null) throw new InvalidOperationException();

            requestWorldState = d;

            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)ClientMessage.RequestWorldState);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }


        public void PlayerCreated(int uniqueId, Vector2 position)
        {
            if (client == null) throw new InvalidOperationException();

            Point pos = position.Round();

            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)ClientMessage.PlayerCreated);
            message.Write(uniqueId);
            message.Write(pos.X);
            message.Write(pos.Y);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }


        public void PlayerMoved(int uniqueId, Vector2 destination)
        {
            if (client == null) throw new InvalidOperationException();

            Point dest = destination.Round();

            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)ClientMessage.PlayerMoved);
            message.Write(uniqueId);
            message.Write(dest.X);
            message.Write(dest.Y);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
