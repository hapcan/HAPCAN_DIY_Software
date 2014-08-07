using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace example6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0x10;
            comboBox2.SelectedIndex = 0x30;
            comboBox3.SelectedIndex = 0xF0;
            comboBox4.SelectedIndex = 0xF0;
            comboBox5.SelectedIndex = 0xFF;
            comboBox6.SelectedIndex = 0xFF;
            comboBox7.SelectedIndex = 0x00;
            comboBox8.SelectedIndex = 0x00;
            comboBox9.SelectedIndex = 0xFF;
            comboBox10.SelectedIndex = 0xFF;
            comboBox11.SelectedIndex = 0xFF;
            comboBox12.SelectedIndex = 0xFF;
        }

        private Socket client;
        private Thread receiveThread;
        private bool receiveThreadON = false;

        delegate void AddTextCallBack(string text);

        public void AddText(string text)
        { 
            if(richTextBox1.InvokeRequired)
            {
                AddTextCallBack t = new AddTextCallBack(AddText);
                this.Invoke(t, new object[] {text} );
            }
            else
            {
                richTextBox1.AppendText(text + Environment.NewLine);
                richTextBox1.ScrollToCaret();
            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                //get the remote endpoint for the socket.
                IPAddress[] IPs = Dns.GetHostAddresses(textBox1.Text);
                IPEndPoint ethModule = new IPEndPoint(IPs[0], Convert.ToInt16(textBox2.Text));
                //create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //connect to the remote endpoint.
                client.Connect(ethModule);
                //start receiving in new thread
                receiveThread = new Thread(SocketReceive);
                receiveThread.IsBackground = true;
                receiveThreadON = true;
                receiveThread.Start();
                //display
                AddText("-----");
                AddText("Connected to " + client.RemoteEndPoint.ToString());
                //other
                Connect.Enabled = false;
                Disconnect.Enabled = true;
                Send.Enabled = true;
            }
            catch (Exception ex)
            {
                AddText(ex.Message);
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                //stop receive thread
                receiveThreadON = false;             
                //disable sends and receives on a socket.
                client.Shutdown(SocketShutdown.Both);
                //close socket connection and release all resources 
                client.Close();
                //display
                AddText("-----");
                AddText("Disconnected");
                //other
                Connect.Enabled = true;
                Disconnect.Enabled = false;
                Send.Enabled = false;
            }
            catch (Exception ex)
            {
                AddText(ex.Message);
            }
        }
        
        private void Send_Click(object sender, EventArgs e)
        {
            byte[] txBytes = new byte[15];

            try
            {
                //form frame
                txBytes[0] = 0xAA;                              //start byte
                txBytes[14] = 0xA5;                             //stop byte
                txBytes[13] = Convert.ToByte(label3.Text, 16);  //check sum
                for (int i = 1; i < 13; i++)                    //data
                    txBytes[i] = (byte)((ComboBox)this.Controls.Find("comboBox" + i.ToString(), true).First()).SelectedIndex;
                //send
                client.Send(txBytes);
                //display
                string txLine = "";
                for (int i = 0; i < 15; i++)
                    txLine += (txBytes[i].ToString("X2") + " ");
                AddText("-----");
                AddText("TX ->  " + txLine);
            }
            catch (Exception ex)
            {
                AddText(ex.Message);
                Disconnect_Click(null,null);
            }
        }

        private void SocketReceive()
        {
            byte[] rxBytes = new byte[15];

            while (receiveThreadON)
            {
                try
                {
                    while (client.Available > 14)
                    {
                        client.Receive(rxBytes, 15, SocketFlags.None);
                        //display
                        string rxLine = "";
                        for (int i = 0; i < 15; i++)
                            rxLine += (rxBytes[i].ToString("X2") + " ");
                        AddText("RX <-  " + rxLine);
                    }
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    AddText(ex.Message);
                }

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //calculate the checksum
            byte checksum = 0;
            for(int i=1; i<13; i++)
                checksum += (byte)((ComboBox)this.Controls.Find("comboBox" + i.ToString(), true).First()).SelectedIndex;
            //display
            label3.Text = checksum.ToString("X2");
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }


    }
}
