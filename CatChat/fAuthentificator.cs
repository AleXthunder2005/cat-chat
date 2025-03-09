using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatChat
{
    public partial class fAuthentificator: Form
    {
        public fAuthentificator()
        {
            InitializeComponent();
            InitializeUser();

            viewUpdate();
        }

        //fields
        private string m_userName;
        private IPAddress m_IP;

        //properties
        public string UserName 
        { 
            get {return m_userName; } 
            set { m_userName = value; } 
        }
        public IPAddress UserIP
        {
            get { return m_IP; }
            set { m_IP = value; }
        }
        //model
        private void InitializeUser() 
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostEntry(hostName).AddressList;
            foreach (IPAddress ip in addresses)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    UserIP = ip;
                    UserName = hostName;
                }
            }
        }

        //view
        private void viewUpdate() 
        {
            tbNameValue.Text = UserName;
            lIPValue.Text = UserIP.ToString();
        }

        //controllers
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            UserName = tbNameValue.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
