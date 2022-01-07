using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace example7;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        //set hardware request to all nodes
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

    //declarations
    private SerialPort serPort;
    private Thread receiveThread;
    private bool receiveThreadON = false;

    public void AddText(string text)
    {
        //request not from ui thread?
        if (this.InvokeRequired)
        {
            //invoke on ui thread
            this.Invoke(() => AddText(text));
        }
        else
        {
            richTextBox1.AppendText(text + Environment.NewLine);
            richTextBox1.ScrollToCaret();
        }
    }

    private void Open_Click(object sender, EventArgs e)
    {
        try
        {
            //create serial port
            serPort = new SerialPort();
            serPort.PortName = comboBox13.Text;
            serPort.BaudRate = 115200;
            serPort.Parity = Parity.None;
            serPort.StopBits = StopBits.One;
            serPort.DataBits = 8;
            serPort.Handshake = Handshake.None;
            serPort.Open();
            //start receiving in new thread
            receiveThread = new Thread(SocketReceive);
            receiveThread.IsBackground = true;
            receiveThreadON = true;
            receiveThread.Start();
            //display
            AddText("-----");
            AddText("Connected to " + serPort.PortName.ToString());
            //other
            OpenPort.Enabled = false;
            ClosePort.Enabled = true;
            Send.Enabled = true;
        }
        catch (Exception ex)
        {
            AddText(ex.Message);
        }
    }

    private void Close_Click(object sender, EventArgs e)
    {
        try
        {
            //stop receive thread
            receiveThreadON = false;
            //close serial port and release all resources 
            serPort.Close();
            //display
            AddText("-----");
            AddText("Disconnected");
            //other
            OpenPort.Enabled = true;
            ClosePort.Enabled = false;
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
            serPort.Write(txBytes, 0, 15);
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
            Close_Click(null, null);
        }
    }

    private void SocketReceive()
    {
        byte[] rxBytes = new byte[15];

        while (receiveThreadON)
        {
            try
            {
                while (serPort.BytesToRead > 14)
                {
                    serPort.Read(rxBytes, 0, 15);
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
                receiveThreadON = false;
            }

        }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //calculate the checksum
        byte checksum = 0;
        for (int i = 1; i < 13; i++)
            checksum += (byte)((ComboBox)this.Controls.Find("comboBox" + i.ToString(), true).First()).SelectedIndex;
        //display
        label3.Text = checksum.ToString("X2");
    }

    private void Clear_Click(object sender, EventArgs e)
    {
        richTextBox1.Clear();
    }
}
