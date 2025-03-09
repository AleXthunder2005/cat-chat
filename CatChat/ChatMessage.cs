using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CatChat
{
    class ChatMessage
    {
        private const byte MESSAGE_TYPE_SIZE = 1;
        private const byte N_SIZE = 4;


        public enum MessageType 
        {
            Message,             //сообщение
            NameTransfer,        //передача имени
            ConnectionNotice,    //уведомление о подключении
            DisconnectionNotice  //уведомление об отключении
        }
        //properties
        public byte[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        private byte[] m_data;   //0 - тип сообщения, 1-4 - длина сообщения, 5-9 - ip отправителя, все остальное - сообщение

        public ChatMessage(MessageType type, IPAddress sender, string message = "") 
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] ipBytes = sender.GetAddressBytes();
            int n = messageBytes.Length + ipBytes.Length;
            byte[] nBytes = BitConverter.GetBytes(n);

            m_data = new byte[5 + n];

            m_data[0] = (byte)type;
            Array.Copy(nBytes, 0, m_data, 1, nBytes.Length);
            Array.Copy(ipBytes, 0, m_data, 1 + 4, ipBytes.Length);
            Array.Copy(messageBytes, 0, m_data, 1 + 4 + 4, messageBytes.Length);
        }

        public MessageType GetMessageType() 
        { 
            return (MessageType)m_data[0]; 
        }
        public int GetDataSize ()
        {
            return m_data.Length - 2;
        }
        public IPAddress GetSenderIP()
        {
            byte[] ipBytes = new byte[4];
            Array.Copy(m_data, 1+4, ipBytes, 0, ipBytes.Length);
            return new IPAddress(ipBytes);
        }
        public string GetMessage()
        {
            int n = BitConverter.ToInt32(m_data, 1);
            byte[] messageBytes = new byte[n - 4];
            
            Array.Copy(m_data, 1 + 4 + 4, messageBytes, 0, messageBytes.Length);
            return Encoding.UTF8.GetString(messageBytes);
        }
    }
}
