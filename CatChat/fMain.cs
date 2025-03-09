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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
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
        private string _userName = null;
        private IPAddress _ip = null;
        private UdpClient _udpClient = null;
        private TcpListener _tcpListener = null;
        private Dictionary<string, TcpClient> _activeNodes = new Dictionary<string, TcpClient>();
        private bool _isRunning = true;


        //properties
        public string UserName
        {
            get { return _userName; }
            private set { _userName = value; }
        }
        public IPAddress UserIP
        {
            get { return _ip; }
            private set { _ip = value; }
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
                    tbLog.AppendText($"{currTime.ToString()}: started chat as {UserName}({UserIP}){NEW_LINE}");
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
            
        }

        private void StartChat() 
        {
            // Запуск прослушивания UDP-пакетов
            _udpClient = new UdpClient(DEFAULT_UDP_PORT);
            _udpClient.EnableBroadcast = true;
            Thread udpThread = new Thread(ListenForUdpBroadcasts);
            udpThread.Start();

            // Запуск TCP-сервера
            _tcpListener = new TcpListener(IPAddress.Any, DEFAULT_TCP_PORT);
            _tcpListener.Start();
            Thread tcpThread = new Thread(ListenForTcpConnections);
            tcpThread.Start();

            SendUDPBroadcastPacket();


            ViewUpdate();
        }

        private void ListenForUdpBroadcasts()
        {
            while (_isRunning)
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpClient.Receive(ref remoteEndpoint);

                ChatMessage receivedMessage = new ChatMessage();
                MessageType type = receivedMessage.GetMessageType();
                switch (type) 
                {
                    case MessageType.ConnectionNotice:
                        LogUpdate(LogMessageType.ConnectionNotice, DateTime.Now, receivedMessage.GetSenderIP());
                        break;
                }




                //EstablishTcpConnection(nodeName, remoteEndpoint.Address);
            }
        }

        private void EstablishTcpConnection(string nodeName, IPAddress ipAddress)
        {
            //TcpClient tcpClient = new TcpClient();
            //tcpClient.Connect(ipAddress, _tcpPort);

            //// Отправляем свое имя для идентификации
            //byte[] nameBytes = Encoding.UTF8.GetBytes(_name);
            //tcpClient.GetStream().Write(nameBytes, 0, nameBytes.Length);

            //_activeNodes[nodeName] = tcpClient;

            //// Запускаем поток для чтения сообщений от этого узла
            //Thread readThread = new Thread(() => ReadMessagesFromNode(nodeName, tcpClient));
            //readThread.Start();
        }

        private void ListenForTcpConnections()
        {
            //while (_isRunning)
            //{
            //    TcpClient tcpClient = _tcpListener.AcceptTcpClient();

            //    // Чтение имени узла
            //    byte[] buffer = new byte[1024];
            //    int bytesRead = tcpClient.GetStream().Read(buffer, 0, buffer.Length);
            //    string nodeName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            //    _activeNodes[nodeName] = tcpClient;

            //    Console.WriteLine($"Установлено TCP-соединение с узлом: {nodeName}");

            //    // Запускаем поток для чтения сообщений от этого узла
            //    Thread readThread = new Thread(() => ReadMessagesFromNode(nodeName, tcpClient));
            //    readThread.Start();
            //}
        }

        private void SendUDPBroadcastPacket()
        {
            int port = 12345;
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.EnableBroadcast = true;
                IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, port);
                ChatMessage notice = new ChatMessage(MessageType.ConnectionNotice, UserIP, UserName);

                udpClient.Send(notice.Data, notice.Data.Length, broadcastEndpoint);
            }
            LogUpdate(LogMessageType.ConnectionNotice, DateTime.Now, UserIP);
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
