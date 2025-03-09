using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CatChat.ChatMessage;
using static CatChat.Node;

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
            Connected,        //мы подключились
            Disconnected,     //мы отключились
            NodeDetected,     //обнаружен узел
            NodeDisconnected, //узел ушел в закат
            MessageSent,      //мы отправили сообщение
            MessageReceived,  //мы получили сообщение
            ConnectionNotice,  //мы послали уведомление что подключились
            DisconnectionNotice  //мы послали уведомление что отключились
        }
        string NEW_LINE = Environment.NewLine;
        const int DEFAULT_UDP_PORT = 11111;
        const int DEFAULT_TCP_PORT = 22222;

        //fields
        private string m_userName = null;
        private IPAddress m_IP = null;


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
                case LogMessageType.ConnectionNotice:
                    tbLog.Text += $"{currTime.ToString()}: connection notice from {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
                case LogMessageType.DisconnectionNotice:
                    tbLog.Text += $"{currTime.ToString()}: disconnection notice from {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
            }
        }
        private void EndChat() 
        {
            SendUDPBroadcastPacket(LogMessageType.DisconnectionNotice);
        }

        private void StartChat() 
        {
            SendUDPBroadcastPacket(LogMessageType.ConnectionNotice);
            ViewUpdate();
        }

        private void SendUDPBroadcastPacket(LogMessageType type)
        {
            int port = 12345;
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.EnableBroadcast = true;
                IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, port);
                ChatMessage notice = new ChatMessage(MessageType.ConnectionNotice, UserIP, UserName);

                udpClient.Send(notice.Data, notice.Data.Length, broadcastEndpoint);
            }
            LogUpdate(type, DateTime.Now, UserIP);
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

                StartChat();
  
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndChat();
            LogUpdate(LogMessageType.Disconnected, DateTime.Now, UserIP);
        }
    }
}
