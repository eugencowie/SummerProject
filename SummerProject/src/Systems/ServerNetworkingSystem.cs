using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class ServerNetworkingSystem : ProcessingSystem
    {
        public static bool IsServer = false;

        NetServer server = null;
        List<string> connections = new List<string>();

        public override void ProcessSystem()
        {
            if (!IsServer)
                return;

            if (server == null)
            {
                server = new NetServer(new NetPeerConfiguration("SummerProject") {
                    MaximumConnections = 10,
                    Port = 14242
                });
            }

            if (server.Status == NetPeerStatus.NotRunning)
            {
                server.Start();

                // TODO: server is client too?
                ClientNetworkingSystem.IsClient = true;
            }

            NetIncomingMessage im;
            while ((im = server.ReadMessage()) != null)
            {
                // Handle incoming message.
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        Console.WriteLine(text);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        string reason = im.ReadString();
                        Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                        if (status == NetConnectionStatus.Connected && im.SenderConnection.RemoteHailMessage != null)
                            Console.WriteLine("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());

                        UpdateConnectionsList();
                        break;

                    case NetIncomingMessageType.Data:
                        // incoming chat message from a client
                        string chat = im.ReadString();

                        Console.WriteLine("Broadcasting '" + chat + "'");

                        // broadcast this to all connections, except sender
                        List<NetConnection> all = server.Connections; // get copy
                        all.Remove(im.SenderConnection);

                        if (all.Count > 0)
                        {
                            NetOutgoingMessage om = server.CreateMessage();
                            om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                            server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        break;

                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                        break;
                }
                server.Recycle(im);
            }
        }

        private void UpdateConnectionsList()
        {
            connections.Clear();

            foreach (NetConnection conn in server.Connections)
            {
                string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                connections.Add(str);
            }
        }
    }
}
