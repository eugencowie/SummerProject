using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SummerProject
{
    enum ServerMessage {
        SetPlayerToRemote,
        MoveRemotePlayer
    }

    enum ClientMessage {
        IsReady,
        PlayerHasMoved
    }

    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class NetworkingSystem : ProcessingSystem
    {
        public static bool IsServer = false;
        public static bool IsClient = false;

        public static NetworkingSystem Instance = null;

        NetServer server = null;
        NetClient client = null;

        public NetworkingSystem()
        {
            Instance = this;
        }

        public override void ProcessSystem()
        {
            if (IsServer)
            {
                // Make sure that the server has been initialised.
                if (server == null)
                {
                    NetPeerConfiguration config = new NetPeerConfiguration("SummerProject");
                    config.Port = 14242;
                    config.MaximumConnections = 10;
                    config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

                    server = new NetServer(config);
                    server.RegisterReceivedCallback(new SendOrPostCallback(Server_OnMessageReceived));
                }

                // Make sure that the server is running.
                if (server.Status == NetPeerStatus.NotRunning)
                    server.Start();
            }

            if (IsClient)
            {
                // Make sure that the client has been initialised.
                if (client == null)
                {
                    NetPeerConfiguration config = new NetPeerConfiguration("SummerProject");
                    config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

                    client = new NetClient(config);
                    client.RegisterReceivedCallback(new SendOrPostCallback(Client_OnMessageReceived));
                }

                // Make sure that the client is connected.
                if (client.Status == NetPeerStatus.NotRunning) {
                    client.Start();
                    client.DiscoverLocalPeers(14242);
                }
            }
        }

        private void Server_OnMessageReceived(object peer)
        {
            NetIncomingMessage message = ((NetPeer)peer).ReadMessage();
            if (message != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        string serverName = Environment.UserDomainName + "\\" + Environment.UserName;
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write(serverName);
                        server.SendDiscoveryResponse(response, message.SenderEndPoint);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                        string reason = message.ReadString();
                        Console.WriteLine("[SERVER] " + NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                        if (status == NetConnectionStatus.Connected && message.SenderConnection.RemoteHailMessage != null)
                            Console.WriteLine("[SERVER] Remote hail: " + message.SenderConnection.RemoteHailMessage.ReadString());
                        //UpdateConnectionsList();
                        break;

                    case NetIncomingMessageType.Data:
                        ClientMessage msgType = (ClientMessage)message.ReadByte();
                        if (msgType == ClientMessage.IsReady) {
                            response = server.CreateMessage();
                            response.Write((byte)ServerMessage.SetPlayerToRemote);
                            server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        if (msgType == ClientMessage.PlayerHasMoved) {
                            int x = message.ReadInt32();
                            int y = message.ReadInt32();
                            // Broadcast this to all connections, except sender.
                            List<NetConnection> all = server.Connections; // get copy
                            all.Remove(message.SenderConnection);
                            if (all.Count > 0) {
                                //NetOutgoingMessage om = server.CreateMessage();
                                //om.Write(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " said: " + data);
                                NetOutgoingMessage om = server.CreateMessage();
                                om.Write((byte)ServerMessage.MoveRemotePlayer);
                                om.Write(x);
                                om.Write(y);
                                server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                        }
                        //string data = message.ReadString();
                        //Console.WriteLine("[SERVER] Broadcasting '" + data + "'");
                        // Broadcast this to all connections, except sender.
                        //List<NetConnection> all = server.Connections; // get copy
                        //all.Remove(message.SenderConnection);
                        //if (all.Count > 0) {
                        //    NetOutgoingMessage om = server.CreateMessage();
                        //    om.Write(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " said: " + data);
                        //    server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                        //}
                        break;

#if DEBUG
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Console.WriteLine("[server-debug] " + text);
                        break;

                    default:
                        Console.WriteLine("[server-debug] Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes " + message.DeliveryMethod + "|" + message.SequenceChannel);
                        break;
#endif
                }

                server.Recycle(message);
            }
        }

        private void Client_OnMessageReceived(object peer)
        {
            NetIncomingMessage message = ((NetPeer)peer).ReadMessage();
            if (message != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        Console.WriteLine("[CLIENT] Found server at " + message.SenderEndPoint + " name: " + message.ReadString());
                        client.Connect(message.SenderEndPoint.Address.ToString(), message.SenderEndPoint.Port);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                        /*if (status == NetConnectionStatus.Connected)
                            s_form.EnableInput();
                        else
                            s_form.DisableInput();
                        if (status == NetConnectionStatus.Disconnected)
                            s_form.button2.Text = "Connect";*/
                        string reason = message.ReadString();
                        Console.WriteLine("[CLIENT] " + status.ToString() + ": " + reason);
                        break;

                    case NetIncomingMessageType.Data:
                        ServerMessage msgType = (ServerMessage)message.ReadByte();
                        if (msgType == ServerMessage.SetPlayerToRemote) {
                            entityWorld.TagManager.GetEntity("player").GetComponent<PlayerInfo>().LocalPlayer = false;
                        }
                        if (msgType == ServerMessage.MoveRemotePlayer) {
                            int x = message.ReadInt32();
                            int y = message.ReadInt32();
                            Entity entity = entityWorld.TagManager.GetEntity("player");
                            if (!entity.GetComponent<PlayerInfo>().LocalPlayer) {
                                // Convert destination from pixel coords to block coords.
                                Entity level = entityWorld.TagManager.GetEntity("level");
                                Tilemap tilemap = level.GetComponent<Tilemap>();
                                int blockSize = tilemap.BlockSize;
                                Vector2 destinationBlock = new Vector2() {
                                    X = (int)Math.Round((float)x / blockSize),
                                    Y = (int)Math.Round((float)y / blockSize),
                                };
                                // If the player is already moving, stop and go to the new destination instead.
                                if (entity.HasComponent<PlayerMoveAction>())
                                    entity.RemoveComponent<PlayerMoveAction>();
                                // The move action tells the movement system to move the player to the specified destination.
                                entity.AddComponent(new PlayerMoveAction() {
                                    Destination = destinationBlock,
                                    Speed = 4.0f
                                });
                            }
                        }
                        //string data = message.ReadString();
                        //Console.WriteLine("[CLIENT] " + data);
                        break;

#if DEBUG
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Console.WriteLine("[client-debug] " + text);
                        break;

                    default:
                        Console.WriteLine("[client-debug] Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes");
                        break;
#endif
                }

                client.Recycle(message);
            }
        }

        public void Client_Send(string text)
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write((byte)ClientMessage.IsReady);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            //Console.WriteLine("[CLIENT] Sending '" + text + "'");
            //client.FlushSendQueue();
        }

        public void Client_SendMoveMessage(int destX, int destY)
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write((byte)ClientMessage.PlayerHasMoved);
            om.Write(destX);
            om.Write(destY);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
