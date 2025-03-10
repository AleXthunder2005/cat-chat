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
            SendMessageToAll();
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

        private void SendUDPBroadcastPacket()
        {
            IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, DEFAULT_UDP_PORT);
            ChatMessage notice = new ChatMessage(MessageType.ConnectionNotice, UserIP, UserName);

            _udpClient.Send(notice.Data, notice.Data.Length, broadcastEndpoint);
            LogUpdate(LogMessageType.ConnectionNotice, DateTime.Now, UserIP);
        }

        private void ListenForUdpBroadcasts()
        {
            while (_isRunning)
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpClient.Receive(ref remoteEndpoint);

                ChatMessage receivedMessage = new ChatMessage();
                receivedMessage.Data = data;
                MessageType type = receivedMessage.GetMessageType();
                switch (type) 
                {
                    case MessageType.ConnectionNotice:
                        LogUpdate(LogMessageType.NodeDetected, DateTime.Now, receivedMessage.GetSenderIP());
                        InitiateTcpConnection(receivedMessage.GetMessage(), receivedMessage.GetSenderIP());
                        break;        
                }  
            }
        }

        private void InitiateTcpConnection(string senderName, IPAddress sender)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(sender, DEFAULT_TCP_PORT);

            // Отправляем свое имя для идентификации
            ChatMessage message = new ChatMessage(MessageType.NameTransfer, UserIP, UserName);
            tcpClient.GetStream().Write(message.Data, 0, message.Data.Length);
            
            //добавили подключившийся узел
            _activeNodes[senderName] = tcpClient;

            // Запускаем поток для чтения сообщений от этого узла
            Thread readThread = new Thread(() => ReadMessagesFromNode(senderName, tcpClient));
            readThread.Start();
        }

        private void ListenForTcpConnections()
        {
            while (_isRunning)
            {
                TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                // Чтение имени узла
                byte[] buffer = new byte[1024];
                int bytesRead = tcpClient.GetStream().Read(buffer, 0, buffer.Length);
                Array.Resize(ref buffer, bytesRead);

                ChatMessage message = new ChatMessage();
                message.Data = buffer;
                if (message.GetMessageType() == MessageType.NameTransfer)
                { 
                    string senderName = message.GetMessage();
                    _activeNodes[senderName] = tcpClient;

                    // Запускаем поток для чтения сообщений от этого узла
                    Thread readThread = new Thread(() => ReadMessagesFromNode(senderName, tcpClient));
                    readThread.Start();
                }
            }
        }

        private void ReadMessagesFromNode(string senderName, TcpClient tcpClient)
        {
            byte[] buffer = new byte[1024];
            NetworkStream stream = tcpClient.GetStream();

            while (_isRunning)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Соединение закрыто

                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);
                    ChatMessage message = new ChatMessage();
                    message.Data = data;

                    switch (message.GetMessageType()) 
                    {
                        case MessageType.Message:
                            LogUpdate(LogMessageType.MessageReceived, DateTime.Now, message.GetSenderIP());
                            tbChat.Text += $"{senderName}: {message.GetMessage()}{NEW_LINE}";
                            break;
                        case MessageType.DisconnectionNotice:
                            LogUpdate(LogMessageType.Disconnected, DateTime.Now, message.GetSenderIP());
                            DisconnectNode(senderName);
                            break;
                    }
                    
                }
                catch
                {
                    break; // Обработка разрыва соединения
                }
            }
            DisconnectNode(senderName);
        }

        private void DisconnectNode(string senderName) 
        {
            _activeNodes.Remove(senderName);
        }

        private void SendMessageToAll(ChatMessage message)
        {
            foreach (var node in _activeNodes)
            {
                try
                {
                    node.Value.GetStream().Write(message.Data, 0, message.Data.Length);
                }
                catch
                {
                    // Обработка ошибок отправки
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _udpClient.Close();
            _tcpListener.Stop();

            foreach (var node in _activeNodes)
            {
                node.Value.Close();
            }
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

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tbMessage.Text != "")
            {
                ChatMessage message = new ChatMessage(MessageType.Message, UserIP, tbMessage.Text);
                tbMessage.Text = "";
                tbChat.Text += $"{UserName}: {message.GetMessage()}{NEW_LINE}";
                SendMessageToAll(message);
            }
        }
    }
}
