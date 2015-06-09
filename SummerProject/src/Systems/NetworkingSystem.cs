using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lidgren.Network;
using System;
using System.Threading;

namespace SummerProject
{
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
                    server.RegisterReceivedCallback(new SendOrPostCallback(HandleMessageOnServer));
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
                    config.AutoFlushSendQueue = false;

                    client = new NetClient(config);
                    client.RegisterReceivedCallback(new SendOrPostCallback(HandleMessageOnClient));
                }

                // Make sure that the client is connected.
                if (client.Status == NetPeerStatus.NotRunning)
                {
                    client.Start();

                    NetOutgoingMessage hail = client.CreateMessage("This is the hail message");
                    client.Connect("127.0.0.1", 14242);
                }
            }
        }

        private void HandleMessageOnServer(object peer)
        {
            NetIncomingMessage message;
            while ((message = server.ReadMessage()) != null)
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
                        string data = message.ReadString();
                        Console.WriteLine("[SERVER] Broadcasting '" + data + "'");
                        // broadcast this to all connections, except sender
                        //List<NetConnection> all = server.Connections; // get copy
                        //all.Remove(im.SenderConnection);
                        //if (all.Count > 0)
                        //{
                        //    NetOutgoingMessage om = server.CreateMessage();
                        //    om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                        //    server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                        //}
                        break;

                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Console.WriteLine("[SERVER] " + text);
                        break;

                    default:
                        Console.WriteLine("[SERVER] Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes " + message.DeliveryMethod + "|" + message.SequenceChannel);
                        break;
                }

                server.Recycle(message);
            }
        }

        private void HandleMessageOnClient(object peer)
        {
            NetIncomingMessage message;
            while ((message = client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
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
                        string data = message.ReadString();
                        Console.WriteLine("[CLIENT] " + data);
                        break;

                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Console.WriteLine("[CLIENT] " + text);
                        break;

                    default:
                        Console.WriteLine("[CLIENT] Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes");
                        break;
                }

                client.Recycle(message);
            }
        }

        public void Send(string text)
        {
            NetOutgoingMessage om = client.CreateMessage(text);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            Console.WriteLine("[CLIENT] Sending '" + text + "'");
            client.FlushSendQueue();
        }
    }
}
