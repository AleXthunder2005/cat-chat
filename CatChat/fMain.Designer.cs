namespace CatChat
{
    partial class fMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pSide = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.lbUsers = new System.Windows.Forms.ListBox();
            this.msMainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.pChat = new System.Windows.Forms.Panel();
            this.tbChat = new System.Windows.Forms.TextBox();
            this.pMessageTypesetter = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.pCredentials = new System.Windows.Forms.Panel();
            this.lChatterCredentials = new System.Windows.Forms.Label();
            this.pSide.SuspendLayout();
            this.msMainMenu.SuspendLayout();
            this.pChat.SuspendLayout();
            this.pMessageTypesetter.SuspendLayout();
            this.pCredentials.SuspendLayout();
            this.SuspendLayout();
            // 
            // pSide
            // 
            this.pSide.BackColor = System.Drawing.Color.SkyBlue;
            this.pSide.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSide.Controls.Add(this.btnExit);
            this.pSide.Controls.Add(this.lbUsers);
            this.pSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pSide.Location = new System.Drawing.Point(0, 28);
            this.pSide.Name = "pSide";
            this.pSide.Size = new System.Drawing.Size(285, 688);
            this.pSide.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Snow;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(3, 642);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(277, 36);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbUsers
            // 
            this.lbUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbUsers.BackColor = System.Drawing.Color.White;
            this.lbUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbUsers.FormattingEnabled = true;
            this.lbUsers.ItemHeight = 16;
            this.lbUsers.Location = new System.Drawing.Point(4, 6);
            this.lbUsers.Name = "lbUsers";
            this.lbUsers.Size = new System.Drawing.Size(275, 628);
            this.lbUsers.TabIndex = 0;
            // 
            // msMainMenu
            // 
            this.msMainMenu.BackColor = System.Drawing.Color.DodgerBlue;
            this.msMainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.msMainMenu.Location = new System.Drawing.Point(0, 0);
            this.msMainMenu.Name = "msMainMenu";
            this.msMainMenu.Size = new System.Drawing.Size(1312, 28);
            this.msMainMenu.TabIndex = 1;
            this.msMainMenu.Text = "Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.conectionToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // conectionToolStripMenuItem
            // 
            this.conectionToolStripMenuItem.Name = "conectionToolStripMenuItem";
            this.conectionToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.conectionToolStripMenuItem.Text = "Connect";
            this.conectionToolStripMenuItem.Click += new System.EventHandler(this.conectionToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(162, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tbLog
            // 
            this.tbLog.BackColor = System.Drawing.Color.White;
            this.tbLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbLog.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbLog.Location = new System.Drawing.Point(285, 580);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(1027, 136);
            this.tbLog.TabIndex = 2;
            // 
            // pChat
            // 
            this.pChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pChat.Controls.Add(this.tbChat);
            this.pChat.Controls.Add(this.pMessageTypesetter);
            this.pChat.Controls.Add(this.pCredentials);
            this.pChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pChat.Location = new System.Drawing.Point(285, 28);
            this.pChat.Name = "pChat";
            this.pChat.Size = new System.Drawing.Size(1027, 552);
            this.pChat.TabIndex = 3;
            // 
            // tbChat
            // 
            this.tbChat.BackColor = System.Drawing.Color.White;
            this.tbChat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbChat.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbChat.Location = new System.Drawing.Point(0, 56);
            this.tbChat.Multiline = true;
            this.tbChat.Name = "tbChat";
            this.tbChat.ReadOnly = true;
            this.tbChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbChat.Size = new System.Drawing.Size(1025, 413);
            this.tbChat.TabIndex = 3;
            // 
            // pMessageTypesetter
            // 
            this.pMessageTypesetter.BackColor = System.Drawing.Color.SkyBlue;
            this.pMessageTypesetter.Controls.Add(this.btnSend);
            this.pMessageTypesetter.Controls.Add(this.tbMessage);
            this.pMessageTypesetter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pMessageTypesetter.Location = new System.Drawing.Point(0, 469);
            this.pMessageTypesetter.Name = "pMessageTypesetter";
            this.pMessageTypesetter.Size = new System.Drawing.Size(1025, 81);
            this.pMessageTypesetter.TabIndex = 2;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.Snow;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSend.Location = new System.Drawing.Point(885, 6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(129, 70);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // tbMessage
            // 
            this.tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMessage.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbMessage.Location = new System.Drawing.Point(5, 6);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(874, 70);
            this.tbMessage.TabIndex = 0;
            // 
            // pCredentials
            // 
            this.pCredentials.BackColor = System.Drawing.Color.SkyBlue;
            this.pCredentials.Controls.Add(this.lChatterCredentials);
            this.pCredentials.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pCredentials.Dock = System.Windows.Forms.DockStyle.Top;
            this.pCredentials.Location = new System.Drawing.Point(0, 0);
            this.pCredentials.Name = "pCredentials";
            this.pCredentials.Size = new System.Drawing.Size(1025, 56);
            this.pCredentials.TabIndex = 1;
            // 
            // lChatterCredentials
            // 
            this.lChatterCredentials.AutoSize = true;
            this.lChatterCredentials.Font = new System.Drawing.Font("Courier New", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lChatterCredentials.Location = new System.Drawing.Point(3, 15);
            this.lChatterCredentials.Name = "lChatterCredentials";
            this.lChatterCredentials.Size = new System.Drawing.Size(166, 27);
            this.lChatterCredentials.TabIndex = 0;
            this.lChatterCredentials.Text = "Chat with: ";
            this.lChatterCredentials.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(1312, 716);
            this.Controls.Add(this.pChat);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.pSide);
            this.Controls.Add(this.msMainMenu);
            this.MainMenuStrip = this.msMainMenu;
            this.Name = "fMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CatChat";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pSide.ResumeLayout(false);
            this.msMainMenu.ResumeLayout(false);
            this.msMainMenu.PerformLayout();
            this.pChat.ResumeLayout(false);
            this.pChat.PerformLayout();
            this.pMessageTypesetter.ResumeLayout(false);
            this.pMessageTypesetter.PerformLayout();
            this.pCredentials.ResumeLayout(false);
            this.pCredentials.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pSide;
        private System.Windows.Forms.MenuStrip msMainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem conectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Panel pChat;
        private System.Windows.Forms.TextBox tbChat;
        private System.Windows.Forms.Panel pMessageTypesetter;
        private System.Windows.Forms.Panel pCredentials;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lChatterCredentials;
        private System.Windows.Forms.ListBox lbUsers;
        private System.Windows.Forms.Button btnExit;
    }
}

