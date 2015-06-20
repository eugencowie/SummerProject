using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace SummerProject
{
    /// <summary>
    /// TODO
    /// </summary>
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class Server : ProcessingSystem
    {
        public static bool Active = false;
        public static Server Instance;

        NetServer server;

        public Server()
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
                if (server == null)
                    return;

                if (server.Status != NetPeerStatus.NotRunning)
                    server.Shutdown("server shutdown requested");

                server = null;
                return;
            }

            // Make sure that the server has been initialised.
            if (server == null)
            {
                NetPeerConfiguration config = new NetPeerConfiguration("SummerProject");
                config.Port = 14242;
                config.MaximumConnections = 10;
                config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

                server = new NetServer(config);
                server.RegisterReceivedCallback(OnMessageReceived);
            }

            // Make sure that the server is running.
            if (server.Status == NetPeerStatus.NotRunning)
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
                    NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
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
        /// Called when the server receives a data message from a client.
        /// </summary>
        private void HandleDataMessage(NetIncomingMessage message)
        {
            NetOutgoingMessage response;

            ClientMessage type = (ClientMessage)message.ReadByte();
            switch (type)
            {
                // An IsReady message is sent to the server by a client when that client is ready
                // to receive data messages.
                case ClientMessage.IsReady:
                    response = server.CreateMessage();
                    response.Write((byte)ServerMessage.CreateRemotePlayer);
                    server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                    break;

                // A PlayerHasMoved message is sent to the server by a client when a player has
                // been moved by that client.
                case ClientMessage.PlayerHasMoved:
                    int x = message.ReadInt32();
                    int y = message.ReadInt32();
                    // Broadcast this to all connections, except sender.
                    List<NetConnection> all = server.Connections; // get copy
                    all.Remove(message.SenderConnection);
                    if (all.Count > 0) {
                        response = server.CreateMessage();
                        response.Write((byte)ServerMessage.MoveRemotePlayer);
                        response.Write(x);
                        response.Write(y);
                        server.SendMessage(response, all, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                    break;
            }
        }
    }
}
