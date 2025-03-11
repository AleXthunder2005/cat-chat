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
        private TcpListener _tcpListener = null;

        private Task _udpListeningTask;
        private Task _tcpListeningTask;
        
        private Dictionary<string, TcpClient> _activeNodes = new Dictionary<string, TcpClient>();
        private Dictionary<string, Task> _activeMessageReaders = new Dictionary<string, Task>();
        private Dictionary<string, CancellationTokenSource> _activeCancellationTokens = new Dictionary<string, CancellationTokenSource>();
        private bool _isRunning = false;

        private CancellationTokenSource _cancellationTokenSourceForListeners;


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
            StopChatAsync();
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

        private void UpdateChat(string message)
        {
                tbChat.Text += message;
        }
        private void SendUDPBroadcastPacket()
        {
            IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, DEFAULT_UDP_PORT);
            ChatMessage notice = new ChatMessage(MessageType.ConnectionNotice, UserIP, UserName);
            _udpClient.Send(notice.Data, notice.Data.Length, broadcastEndpoint);

            LogUpdate(LogMessageType.ConnectionNotice, DateTime.Now, UserIP);
        }

        //------------------------------------------------------------------------
        private async void StartChat()
        {
            _isRunning = true;

            // Запуск прослушивания UDP-пакетов
            _udpClient = new UdpClient(new IPEndPoint(UserIP, DEFAULT_UDP_PORT));
            _udpClient.EnableBroadcast = true;

            // Запуск TCP-сервера
            _tcpListener = new TcpListener(new IPEndPoint(UserIP, DEFAULT_TCP_PORT));
            _tcpListener.Start();

            _cancellationTokenSourceForListeners = new CancellationTokenSource();

            _udpListeningTask = Task.Run(() => ListenForUdpBroadcasts(_cancellationTokenSourceForListeners.Token));
            _tcpListeningTask = Task.Run(() => ListenForTcpConnections(_cancellationTokenSourceForListeners.Token));

            // Отправляем UDP-широковещательный пакет
            SendUDPBroadcastPacket();

            ViewUpdate();
        }

        private async Task ListenForUdpBroadcasts(CancellationToken cancelationToken)
        {
            UdpReceiveResult result;
            cancelationToken.Register(() => StopChatAsync());

            while (_isRunning && !cancelationToken.IsCancellationRequested)
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, DEFAULT_UDP_PORT);
                result = await _udpClient.ReceiveAsync();

                if (result.RemoteEndPoint.Address.Equals(UserIP))
                    continue; // Игнорируем собственные пакеты

                ChatMessage receivedMessage = new ChatMessage();
                receivedMessage.Data = result.Buffer;
                MessageType type = receivedMessage.GetMessageType();

                switch (type)
                {
                    case MessageType.ConnectionNotice:
                        LogUpdate(LogMessageType.NodeDetected, DateTime.Now, receivedMessage.GetSenderIP(), receivedMessage.GetMessage());
                        InitiateTcpConnection(receivedMessage.GetMessage(), receivedMessage.GetSenderIP());
                        break;
                }
            }
        }

        //после получения уведомления о подключении
        private void InitiateTcpConnection(string senderName, IPAddress sender)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(sender, DEFAULT_TCP_PORT);
            
            // Отправляем свое имя для идентификации
            ChatMessage message = new ChatMessage(MessageType.NameTransfer, UserIP, UserName);
            tcpClient.GetStream().Write(message.Data, 0, message.Data.Length);
            LogUpdate(LogMessageType.TransferName, DateTime.Now, sender);

            //добавили подключившийся узел
            _activeNodes[senderName] = tcpClient;
            ViewUpdate();

            // Запускаем задачу для чтения сообщений от этого узла
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            _activeMessageReaders[senderName] = Task.Run(() => ReadMessagesFromNode(senderName, tcpClient, token));
            _activeCancellationTokens[senderName] = cancellationTokenSource;
        }

        private async Task ListenForTcpConnections(CancellationToken cancelationToken)
        {
            cancelationToken.Register(() => StopChatAsync());
            while (_isRunning && !cancelationToken.IsCancellationRequested)
            {
                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();

                // Чтение имени узла
                byte[] buffer = new byte[1024];
                int bytesRead = await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    Array.Resize(ref buffer, bytesRead);

                    ChatMessage message = new ChatMessage();
                    message.Data = buffer;
                    if (message.GetMessageType() == MessageType.NameTransfer)
                    {
                        string senderName = message.GetMessage();
                        IPAddress senderIP = message.GetSenderIP();
                        LogUpdate(LogMessageType.NodeDetected, DateTime.Now, senderIP, senderName);

                        _activeNodes[senderName] = tcpClient;
                        ViewUpdate();

                        // Запускаем задачу для чтения сообщений от узла
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        CancellationToken token = cancellationTokenSource.Token;
                        _activeMessageReaders[senderName] = Task.Run(() => ReadMessagesFromNode(senderName, tcpClient, token));
                        _activeCancellationTokens[senderName] = cancellationTokenSource;
                    }
                }
            }
        }

        private async void StopChatAsync()
        {
            LogUpdate(LogMessageType.DisconnectionNotice, DateTime.Now, UserIP);
            ChatMessage message = new ChatMessage(MessageType.DisconnectionNotice, UserIP, UserName);
            SendMessageToAll(message);

            _isRunning = false;
            _cancellationTokenSourceForListeners.Dispose();

            // Дожидаемся завершения задач
            if (_udpListeningTask != null)  //прервать выполнение таски
                await _udpListeningTask;
            if (_tcpListeningTask != null)
                await _tcpListeningTask;

            // Закрытие UDP и TCP сокетов
            _udpClient.Close();
            _tcpListener.Stop();

            // Закрытие всех активных подключений
            foreach (var node in _activeNodes)
            {
                DisconnectNode(node.Key);  //там надо прервать выполнение таски
            }
            _activeNodes.Clear();
            _activeMessageReaders.Clear();

            ViewUpdate();
            tbChat.Text = "";
        }

        private void DisconnectNode(string senderName)
        {
            if (_activeNodes.TryGetValue(senderName, out var client))
            {
                client.Close();
                _activeNodes.Remove(senderName);
                _activeCancellationTokens[senderName].Cancel();
                _activeMessageReaders.Remove(senderName);
                _activeCancellationTokens.Remove(senderName);
            }
            ViewUpdate();
        }

        private async Task ReadMessagesFromNode(string senderName, TcpClient tcpClient, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];
            NetworkStream stream = tcpClient.GetStream();
            cancellationToken.Register(() => StopChatAsync());

            try
            {
                while (_isRunning)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break; // Соединение закрыто

                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);
                    ChatMessage message = new ChatMessage();
                    message.Data = data;

                    switch (message.GetMessageType())
                    {
                        case MessageType.Message:
                            LogUpdate(LogMessageType.MessageReceived, DateTime.Now, message.GetSenderIP(), senderName);
                            UpdateChat($"{senderName}: {message.GetMessage()}{NEW_LINE}");
                            break;
                        case MessageType.DisconnectionNotice:
                            LogUpdate(LogMessageType.NodeDisconnected, DateTime.Now, message.GetSenderIP(), message.GetMessage());
                            DisconnectNode(senderName);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // Обработка ошибок чтения
            }
            finally
            {
                DisconnectNode(senderName);
            }
        }

        //-------------------------------------------------------------------------------------
        private void SendMessageToAll(ChatMessage message)
        {
            foreach (var node in _activeNodes)
            {
                try
                {
                    var stream = node.Value.GetStream();
                    stream.Write(message.Data, 0, message.Data.Length);
                }
                catch
                {
                    // Обработка ошибок отправки
                }
            }
            if (message.GetMessageType() == MessageType.Message)
                LogUpdate(LogMessageType.MessageSent, DateTime.Now, null);
        }

        //view
        private void ViewUpdate()
        {
            //tbMessage
            tbMessage.Text = "";

            //lbUsers
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
                LogUpdate(LogMessageType.Connected, DateTime.Now, UserIP);

                StartChat();
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopChatAsync();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tbMessage.Text != "")
            {
                ChatMessage message = new ChatMessage(MessageType.Message, UserIP, tbMessage.Text);
                tbMessage.Text = "";
                UpdateChat($"{UserName}: {message.GetMessage()}{NEW_LINE}");
                SendMessageToAll(message);
            }
        }
    }
}