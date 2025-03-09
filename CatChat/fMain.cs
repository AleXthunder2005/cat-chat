using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatChat
{
    public partial class fMain: Form
    {
        public fMain()
        {
            InitializeComponent();
        }

        private enum LogMessageType
        {
            Connected,
            Disconnected,
            NodeDetected,
            NodeDisconnected,
            MessageSent,
            MessageReceived
        }
        string NEW_LINE = Environment.NewLine;

        //fields
        private string m_userName = null;
        private IPAddress m_IP = null;

        private string m_log;

        //properties
        public string UserName
        {
            get { return m_userName; }
            private set { m_userName = value; }
        }
        public IPAddress UserIP
        {
            get { return m_IP; }
            private set { m_IP = value; }
        }



        //model
        private void CloseApplication() 
        {
            this.Close();
        }

        private void InitializeUser(string name, IPAddress ip) 
        {
            UserName = name;
            UserIP = ip;
        }

        private void LogUpdate(LogMessageType type, DateTime currTime, IPAddress ip)
        {
            switch (type)
            {
                case LogMessageType.Connected:
                    tbLog.Text += $"{currTime.ToString()}: started chat as {UserName}({UserIP}){NEW_LINE}";
                    break;
                case LogMessageType.Disconnected:
                    tbLog.Text += $"{currTime.ToString()}: ended chat as {UserName}({UserIP}){NEW_LINE}";
                    break;
                case LogMessageType.NodeDetected:
                    tbLog.Text += $"{currTime.ToString()}: user {UserName}({UserIP}) detected{NEW_LINE}";
                    break;
                case LogMessageType.NodeDisconnected:
                    tbLog.Text += $"{currTime.ToString()}: user {UserName}({UserIP}) disconnected{NEW_LINE}";
                    break;
                case LogMessageType.MessageSent:
                    tbLog.Text += $"{currTime.ToString()}: message to {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
                case LogMessageType.MessageReceived:
                    tbLog.Text += $"{currTime.ToString()}: message from {UserName}({UserIP}) received{NEW_LINE}";
                    break;
            }
        }
        private void DisconectChat() 
        { 
        
        }

        //view
        private void ViewUpdate() 
        { 
            
        }

        //controllers
        private void btnExit_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void conectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAuthentificator authentificator = new fAuthentificator();
            if (authentificator.ShowDialog() == DialogResult.OK) 
            {
                InitializeUser(authentificator.UserName, authentificator.UserIP);
                LogUpdate(LogMessageType.Connected, DateTime.Now, UserIP);
                ViewUpdate();
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisconectChat();
            LogUpdate(LogMessageType.Disconnected, DateTime.Now, UserIP);
        }
    }
}
