namespace PCSDKApplication
{
    partial class TCPServerWindow
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
            this.Progress1 = new System.Windows.Forms.Label();
            this.sendMsgBtn = new System.Windows.Forms.Button();
            this.TCPMessageBox = new System.Windows.Forms.TextBox();
            this.clientMessages = new System.Windows.Forms.TextBox();
            this.SendData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Progress1
            // 
            this.Progress1.AutoSize = true;
            this.Progress1.Location = new System.Drawing.Point(359, 61);
            this.Progress1.Name = "Progress1";
            this.Progress1.Size = new System.Drawing.Size(70, 13);
            this.Progress1.TabIndex = 0;
            this.Progress1.Text = "Connecting...";
            // 
            // sendMsgBtn
            // 
            this.sendMsgBtn.Location = new System.Drawing.Point(316, 164);
            this.sendMsgBtn.Name = "sendMsgBtn";
            this.sendMsgBtn.Size = new System.Drawing.Size(157, 81);
            this.sendMsgBtn.TabIndex = 1;
            this.sendMsgBtn.Text = "Send Message";
            this.sendMsgBtn.UseVisualStyleBackColor = true;
            this.sendMsgBtn.Click += new System.EventHandler(this.sendMsgBtn_Click);
            // 
            // TCPMessageBox
            // 
            this.TCPMessageBox.Location = new System.Drawing.Point(247, 138);
            this.TCPMessageBox.Name = "TCPMessageBox";
            this.TCPMessageBox.Size = new System.Drawing.Size(295, 20);
            this.TCPMessageBox.TabIndex = 2;
            // 
            // clientMessages
            // 
            this.clientMessages.Location = new System.Drawing.Point(247, 336);
            this.clientMessages.Name = "clientMessages";
            this.clientMessages.Size = new System.Drawing.Size(295, 20);
            this.clientMessages.TabIndex = 3;
            // 
            // SendData
            // 
            this.SendData.Location = new System.Drawing.Point(284, 362);
            this.SendData.Name = "SendData";
            this.SendData.Size = new System.Drawing.Size(213, 32);
            this.SendData.TabIndex = 4;
            this.SendData.Text = "Send Data";
            this.SendData.UseVisualStyleBackColor = true;
            this.SendData.Click += new System.EventHandler(this.SendData_Click);
            // 
            // TCPServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 416);
            this.Controls.Add(this.SendData);
            this.Controls.Add(this.clientMessages);
            this.Controls.Add(this.TCPMessageBox);
            this.Controls.Add(this.sendMsgBtn);
            this.Controls.Add(this.Progress1);
            this.Name = "TCPServerWindow";
            this.Text = "TCP Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Progress1;
        private System.Windows.Forms.Button sendMsgBtn;
        private System.Windows.Forms.TextBox TCPMessageBox;
        private System.Windows.Forms.TextBox clientMessages;
        private System.Windows.Forms.Button SendData;
    }
}