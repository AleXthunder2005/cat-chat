using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatChat
{
    public class Node
    {
        private string _name;
        private IPAddress _ip;
        private int _udpPort;
        private int _tcpPort;
        private UdpClient _udpClient;
        private TcpListener _tcpListener;
        private Dictionary<string, TcpClient> _activeNodes = new Dictionary<string, TcpClient>();
        private bool _isRunning = true;

        public Node(string name, IPAddress ip, int udpPort, int tcpPort)
        {
            _name = name;
            _ip = ip;
            _udpPort = udpPort;
            _tcpPort = tcpPort;
        }

        public void Start()
        {
            // Запуск прослушивания UDP-пакетов
            _udpClient = new UdpClient(_udpPort);
            _udpClient.EnableBroadcast = true;
            Thread udpThread = new Thread(ListenForUdpBroadcasts);
            udpThread.Start();

            // Запуск TCP-сервера
            _tcpListener = new TcpListener(IPAddress.Any, _tcpPort);
            _tcpListener.Start();
            Thread tcpThread = new Thread(ListenForTcpConnections);
            tcpThread.Start();

            // Отправка широковещательного UDP-пакета
            SendBroadcast();

            Console.WriteLine($"Узел {_name} запущен. Ожидание подключений...");
        }

        private void SendBroadcast()
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(_name);
            IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, _udpPort);
            _udpClient.Send(nameBytes, nameBytes.Length, broadcastEndpoint);
        }

        private void ListenForUdpBroadcasts()
        {
            while (_isRunning)
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpClient.Receive(ref remoteEndpoint);
                string nodeName = Encoding.UTF8.GetString(data);

                if (!_activeNodes.ContainsKey(nodeName))
                {
                    Console.WriteLine($"Обнаружен новый узел: {nodeName}");
                    EstablishTcpConnection(nodeName, remoteEndpoint.Address);
                }
            }
        }

        private void EstablishTcpConnection(string nodeName, IPAddress ipAddress)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(ipAddress, _tcpPort);

            // Отправляем свое имя для идентификации
            byte[] nameBytes = Encoding.UTF8.GetBytes(_name);
            tcpClient.GetStream().Write(nameBytes, 0, nameBytes.Length);

            _activeNodes[nodeName] = tcpClient;

            // Запускаем поток для чтения сообщений от этого узла
            Thread readThread = new Thread(() => ReadMessagesFromNode(nodeName, tcpClient));
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
                string nodeName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                _activeNodes[nodeName] = tcpClient;

                Console.WriteLine($"Установлено TCP-соединение с узлом: {nodeName}");

                // Запускаем поток для чтения сообщений от этого узла
                Thread readThread = new Thread(() => ReadMessagesFromNode(nodeName, tcpClient));
                readThread.Start();
            }
        }

        private void ReadMessagesFromNode(string nodeName, TcpClient tcpClient)
        {
            byte[] buffer = new byte[1024];
            NetworkStream stream = tcpClient.GetStream();

            while (_isRunning)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Соединение закрыто

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"{nodeName}: {message}");
                }
                catch
                {
                    break; // Обработка разрыва соединения
                }
            }

            // Удаляем узел из списка активных
            _activeNodes.Remove(nodeName);
            Console.WriteLine($"Узел {nodeName} отключен.");
        }

        public void SendMessageToAll(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            foreach (var node in _activeNodes)
            {
                try
                {
                    node.Value.GetStream().Write(messageBytes, 0, messageBytes.Length);
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
    }
}
