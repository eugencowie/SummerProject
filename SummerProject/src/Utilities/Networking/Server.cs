using Lidgren.Network;
using System;

namespace SummerProject
{
    class Server
    {
        NetServer server;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Server()
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


        public void CheckIsRunning()
        {
            // Make sure that the server is running.
            if (server.Status == NetPeerStatus.NotRunning)
                server.Start();
        }
    }
}
