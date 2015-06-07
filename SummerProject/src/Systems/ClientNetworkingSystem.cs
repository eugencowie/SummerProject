using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lidgren.Network;
using System;
using System.Threading;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class ClientNetworkingSystem : ProcessingSystem
    {
        public static ClientNetworkingSystem Instance;
        public static bool IsClient = false;

        NetClient client = null;

        public ClientNetworkingSystem()
        {
            Instance = this;
        }

        public override void ProcessSystem()
        {
            if (!IsClient)
                return;

            if (client == null)
            {
                client = new NetClient(new NetPeerConfiguration("SummerProject") {
                    AutoFlushSendQueue = false
                });
                client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            }

            if (client.Status == NetPeerStatus.NotRunning)
            {
                client.Start();
                NetOutgoingMessage hail = client.CreateMessage("This is the hail message");
                client.Connect(host: "127.0.0.1", port: 14242);
            }
        }

        public void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = client.ReadMessage()) != null)
            {
                // handle incoming message
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

                        /*if (status == NetConnectionStatus.Connected)
                            s_form.EnableInput();
                        else
                            s_form.DisableInput();

                        if (status == NetConnectionStatus.Disconnected)
                            s_form.button2.Text = "Connect";*/

                        string reason = im.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string chat = im.ReadString();
                        Console.WriteLine(chat);
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                client.Recycle(im);
            }
        }

        public void Send(string text)
        {
            NetOutgoingMessage om = client.CreateMessage(text);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            Console.WriteLine("Sending '" + text + "'");
            client.FlushSendQueue();
        }
    }
}
