using Lidgren.Network;
using System;
using System.Windows.Forms;

namespace SummerProjectServer
{
    public partial class MainWindow : Form
    {
        Network network;

        public MainWindow()
        {
            InitializeComponent();

            network = new Network(this);

            network.Start(
                appId: "SummerProject",
                port: 14242,
                maximumConnections: 10);

            textBox.AppendText("Server started!" + "\r\n");
            textBox.AppendText("Waiting for connections..." + "\r\n" + "\r\n");
        }

        private void tickTimer_Tick(object sender, EventArgs e)
        {
            network.Update();
        }
    }
}
