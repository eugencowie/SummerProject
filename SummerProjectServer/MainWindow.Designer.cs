namespace SummerProjectServer
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.connectionsLabel = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.tickTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // connectionsLabel
            // 
            this.connectionsLabel.AutoSize = true;
            this.connectionsLabel.Location = new System.Drawing.Point(13, 9);
            this.connectionsLabel.Name = "connectionsLabel";
            this.connectionsLabel.Size = new System.Drawing.Size(78, 13);
            this.connectionsLabel.TabIndex = 0;
            this.connectionsLabel.Text = "Connections: 0";
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Location = new System.Drawing.Point(16, 29);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(306, 320);
            this.textBox.TabIndex = 0;
            // 
            // tickTimer
            // 
            this.tickTimer.Interval = 16;
            this.tickTimer.Tick += new System.EventHandler(this.tickTimer_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 361);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.connectionsLabel);
            this.Name = "MainWindow";
            this.Text = "Summer Project (Server)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Timer tickTimer;
        public System.Windows.Forms.Label connectionsLabel;
    }
}

