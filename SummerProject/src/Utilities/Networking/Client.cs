using Lidgren.Network;
using System;
using System.Net;

namespace SummerProject
{
    class DiscoveryResponseEventArgs : EventArgs
    {
        public string HostName { get; private set; }
        public IPEndPoint HostEndPoint { get; private set; }

        public DiscoveryResponseEventArgs(string hostName, IPEndPoint hostEndPoint)
        {
            HostName = hostName;
            HostEndPoint = hostEndPoint;
        }
    }

    class ConnectedToHostEventArgs : EventArgs
    {
    }

    class Client
    {
        public event EventHandler<DiscoveryResponseEventArgs> DiscoveryResponse;
        public event EventHandler<ConnectedToHostEventArgs> ConnectedToHost;
        
        NetClient client;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Client()
        {
            var config = new NetPeerConfiguration("SummerProject");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.RegisterReceivedCallback(OnMessageReceived);
            client.Start();
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
                    if (DiscoveryResponse != null)
                        DiscoveryResponse(this, new DiscoveryResponseEventArgs(serverName, message.SenderEndPoint));
                    break;

                // A StatusChanged message to sent to the client when the status of the connection
                // between the client and server changes.
                case NetIncomingMessageType.StatusChanged:
                    var status = (NetConnectionStatus)message.ReadByte();
                    string reason = message.ReadString();
                    Console.WriteLine("[CLIENT] " + status + ": " + reason);
                    if (status == NetConnectionStatus.Connected)
                        if (ConnectedToHost != null)
                            ConnectedToHost(this, new ConnectedToHostEventArgs());
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


        public void DiscoverLocalPeers(int port)
        {
            client.DiscoverLocalPeers(port);
        }


        public void DiscoverRemotePeers(string host, int port)
        {
            client.DiscoverKnownPeer(host, port);
        }


        public void Connect(string host, int port)
        {
            client.Connect(host, port);
        }
    }
}
