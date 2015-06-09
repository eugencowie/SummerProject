using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SummerProjectServer
{
    enum ClientMessage {
        Connect,
        Disconnect,
        Move
    }

    enum ServerMessage {
        ConnectionDenied,
        ClientConnected,
        ClientDisconnected
    }

    class Network
    {
        MainWindow mainWindow;
        NetServer server;

        List<Connection> clients = new List<Connection>();

        public Network(MainWindow window)
        {
            mainWindow = window;
        }

        public void Start(string appId, int? port=null, int? maximumConnections=null)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(appId);
            if (port.HasValue) config.Port = port.Value;
            if (maximumConnections.HasValue) config.MaximumConnections = maximumConnections.Value;

            // Enable DiscoveryRequest messages.
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            server = new NetServer(config);
            server.Start();
        }

        public void Update()
        {
            UpdateServer();
            UpdateClients();
        }

        private void UpdateServer()
        {
            NetIncomingMessage msgIn;

            while ((msgIn = server.ReadMessage()) != null)
            {
                if (msgIn.MessageType == NetIncomingMessageType.DiscoveryRequest)
                {
                    NetOutgoingMessage response = server.CreateMessage();

                    string serverName = Environment.UserDomainName + "\\" + Environment.UserName;
                    response.Write(serverName);

                    server.SendDiscoveryResponse(response, msgIn.SenderEndPoint);
                }

                if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    switch ((ClientMessage)msgIn.ReadByte())
                    {
                        case ClientMessage.Connect:
                            HandleConnectMessage(msgIn);
                            break;

                        case ClientMessage.Disconnect:
                            HandleDisconnectMessage(msgIn);
                            break;

                        case ClientMessage.Move:
                            HandleMoveMessage(msgIn);
                            break;
                    }
                }

                server.Recycle(msgIn);
            }
        }

        private void UpdateClients()
        {
            if (server.ConnectionsCount == clients.Count) //If the number of the player object actually corresponds to the number of connected clients.
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].Timeout++; //This data member continuously counts up with every frame/tick.

                    //The server simply always sends data to the all players current position of all clients.
                    NetOutgoingMessage msgOut = server.CreateMessage();

                    msgOut.Write("move");
                    msgOut.Write(clients[i].Name);
                    msgOut.Write((int)clients[i].Position.X);
                    msgOut.Write((int)clients[i].Position.Y);

                    server.SendMessage(msgOut, server.Connections, NetDeliveryMethod.Unreliable, 0);

                    if (clients[i].Timeout > 180) //If this is true, so that is the player not sent information with himself
                    {
                        // The procedure here is much the same as the "disconnect" message.

                        server.Connections[i].Disconnect("connection timed out");
                        System.Threading.Thread.Sleep(100);
                        mainWindow.textBox.AppendText(clients[i].Name + " is timed out." + "\r\n");

                        if (server.ConnectionsCount != 0)
                        {
                            // Notify all connected clients.
                            msgOut = server.CreateMessage();
                            msgOut.Write((byte)ServerMessage.ClientDisconnected);
                            msgOut.Write(clients[i].Name);
                            server.SendMessage(msgOut, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }

                        clients.RemoveAt(i);
                        i--;
                        mainWindow.connectionsLabel.Text = "Connections: " + clients.Count;
                        break;
                    }
                }
            }
        }

        private void HandleConnectMessage(NetIncomingMessage msgIn)
        {
            string name = msgIn.ReadString();
            int x = msgIn.ReadInt32();
            int y = msgIn.ReadInt32();

            bool validConnection = true;

            // Check to see if there is another connection with the same name...
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Name.Equals(name))
                {
                    NetOutgoingMessage msgOut = server.CreateMessage();
                    msgOut.Write((byte)ServerMessage.ConnectionDenied);

                    server.SendMessage(msgOut, msgIn.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                    // Make sure the message is sent to the client before disconnecting with them.
                    System.Threading.Thread.Sleep(100);
                    msgIn.SenderConnection.Disconnect("connection denied: name already in use");
                    validConnection = false;
                    break;
                }
            }

            if (validConnection)
            {
                System.Threading.Thread.Sleep(100);
                clients.Add(new Connection(name, new Vector2(x, y), 0));
                mainWindow.textBox.AppendText(name + " connected." + "\r\n");

                for (int i = 0; i < clients.Count; i++)
                {
                    NetOutgoingMessage msgOut = server.CreateMessage();

                    msgOut.Write((byte)ServerMessage.ClientConnected);
                    msgOut.Write(clients[i].Name);
                    msgOut.Write((int)clients[i].Position.X);
                    msgOut.Write((int)clients[i].Position.Y);

                    // Notify all connected clients.
                    server.SendMessage(msgOut, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }

            mainWindow.connectionsLabel.Text = "Connections: " + clients.Count;
        }

        private void HandleDisconnectMessage(NetIncomingMessage msgIn)
        {
            string name = msgIn.ReadString();

            for (int i = 0; i < clients.Count; i++)
            {
                // Find the correct client to disconnect.
                if (clients[i].Name.Equals(name))
                {
                    server.Connections[i].Disconnect("client disconnect");
                    System.Threading.Thread.Sleep(100);
                    mainWindow.textBox.AppendText(name + " disconnected." + "\r\n");

                    if (server.ConnectionsCount != 0)
                    {
                        // Notify all connected clients.
                        NetOutgoingMessage msgOut = server.CreateMessage();
                        msgOut.Write((byte)ServerMessage.ClientDisconnected);
                        msgOut.Write(name);
                        server.SendMessage(msgOut, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                    }

                    clients.RemoveAt(i);
                    i--;
                    break;
                }
            }

            mainWindow.connectionsLabel.Text = "Connections: " + clients.Count;
        }

        private void HandleMoveMessage(NetIncomingMessage msgIn)
        {
            // This message is sent using plain UDP (NetDeliveryMethod.Unreliable) since
            // the message is not required to reach clients in every time. Therefore, it
            // is possible that we may receive an incomplete message, hence the try block.
            try
            {
                string name = msgIn.ReadString();
                int x = msgIn.ReadInt32();
                int y = msgIn.ReadInt32();

                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].Name.Equals(name))
                    {
                        // Update position.
                        clients[i].Position = new Vector2(x, y);

                        // Reset timeout.
                        clients[i].Timeout = 0;
                        break;
                    }
                }
            }
            catch { return; }
        }
    }
}
