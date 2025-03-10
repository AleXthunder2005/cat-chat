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
    public partial class fMain : Form
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
            DisconnectionNotice,  //мы послали уведомление что отключились
            TransferName        //передача имени
        }
        string NEW_LINE = Environment.NewLine;
        const int DEFAULT_UDP_PORT = 28085;
        const int DEFAULT_TCP_PORT = 12105;

        //fields
        private string _userName = null;
        private IPAddress _ip = null;
        private UdpClient _udpClient = null;
        private Thread _udpThread = null;
        private Thread _tcpThread = null;
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

        private void LogUpdate(LogMessageType type, DateTime currTime, IPAddress ip, string name = null)
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
                    tbLog.Text += $"{currTime.ToString()}: user {name}({ip}) detected{NEW_LINE}";
                    break;
                case LogMessageType.NodeDisconnected:
                    tbLog.Text += $"{currTime.ToString()}: user {name}({ip}) disconnected{NEW_LINE}";
                    break;
                case LogMessageType.MessageSent:
                    tbLog.Text += $"{currTime.ToString()}: message from {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
                case LogMessageType.MessageReceived:
                    tbLog.Text += $"{currTime.ToString()}: message from {name}({ip}) received{NEW_LINE}";
                    break;
                case LogMessageType.ConnectionNotice:
                    tbLog.Text += $"{currTime.ToString()}: connection notice from {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
                case LogMessageType.DisconnectionNotice:
                    tbLog.Text += $"{currTime.ToString()}: disconnection notice from {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
                case LogMessageType.TransferName:
                    tbLog.Text += $"{currTime.ToString()}: transfer name from {UserName}({UserIP}) sent{NEW_LINE}";
                    break;
            }
        }

        private void SafeLogUpdate(LogMessageType type, DateTime currTime, IPAddress ip, string name = null)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.Invoke(new Action(() => LogUpdate(type, currTime, ip, name)));
            }
            else
            {
                LogUpdate(type, currTime, ip, name);
            }
        }

        private void SafeUpdateChat(string message)
        {
            if (tbChat.InvokeRequired)
            {
                tbChat.Invoke(new Action(() => tbChat.Text += message));
            }
            else
            {
                tbChat.Text += message;
            }
        }

        private void StartChat()
        {
            _isRunning = true;
            // Запуск прослушивания UDP-пакетов
            _udpClient = new UdpClient(new IPEndPoint(UserIP, DEFAULT_UDP_PORT));
            _udpClient.EnableBroadcast = true;

            _udpThread = new Thread(ListenForUdpBroadcasts);
            _udpThread.Start();

            // Запуск TCP-сервера
            _tcpListener = new TcpListener(new IPEndPoint(UserIP, DEFAULT_TCP_PORT));
            _tcpListener.Start();

            _tcpThread = new Thread(ListenForTcpConnections);
            _tcpThread.Start();

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
        private void EndChat()
        {
            ChatMessage message = new ChatMessage(MessageType.DisconnectionNotice, UserIP, UserName);
            LogUpdate(LogMessageType.DisconnectionNotice, DateTime.Now, UserIP);
            SendMessageToAll(message);
            Stop();
        }
        private void Stop()
        {
            _isRunning = false; // Останавливаем поток

            _udpThread.Join(); // Ожидаем завершения потока
            _tcpThread.Join();

            _udpClient.Close();
            _tcpListener.Stop();

            foreach (var node in _activeNodes)
            {
                node.Value.Close();
            }
        }


        private void ListenForUdpBroadcasts()
        {
            while (_isRunning)
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, DEFAULT_UDP_PORT);
                byte[] data = _udpClient.Receive(ref remoteEndpoint);

                if (remoteEndpoint.Address.Equals(UserIP)) continue; //Если пакет получен от отправителя - игнорируем

                ChatMessage receivedMessage = new ChatMessage();
                receivedMessage.Data = data;
                MessageType type = receivedMessage.GetMessageType();
                switch (type)
                {
                    case MessageType.ConnectionNotice:
                        SafeLogUpdate(LogMessageType.NodeDetected, DateTime.Now, receivedMessage.GetSenderIP(), receivedMessage.GetMessage());
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
            SafeLogUpdate(LogMessageType.TransferName, DateTime.Now, sender);

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
                    IPAddress senderIP = message.GetSenderIP();
                    SafeLogUpdate(LogMessageType.NodeDetected, DateTime.Now, senderIP, senderName);
                    _activeNodes[senderName] = tcpClient;
                    ViewUpdate();

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
                            SafeLogUpdate(LogMessageType.MessageReceived, DateTime.Now, message.GetSenderIP(), senderName);
                            SafeUpdateChat($"{senderName}: {message.GetMessage()}{NEW_LINE}");
                            break;
                        case MessageType.DisconnectionNotice:
                            SafeLogUpdate(LogMessageType.NodeDisconnected, DateTime.Now, message.GetSenderIP(), message.GetMessage());

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
            SafeLogUpdate(LogMessageType.MessageSent, DateTime.Now, null);
        }



        //view
        private void ViewUpdate()
        {
            lbUsers.Items.Clear();
            foreach (var node in _activeNodes) 
            { 
                lbUsers.Items.Add(node.Key);
            }
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
                SafeLogUpdate(LogMessageType.Connected, DateTime.Now, UserIP);

                StartChat();
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndChat();
            SafeLogUpdate(LogMessageType.Disconnected, DateTime.Now, UserIP);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tbMessage.Text != "")
            {
                ChatMessage message = new ChatMessage(MessageType.Message, UserIP, tbMessage.Text);
                tbMessage.Text = "";
                SafeUpdateChat($"{UserName}: {message.GetMessage()}{NEW_LINE}");
                SendMessageToAll(message);
            }
        }
    }
}