using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace NixeSync
{
    public partial class Form1 : Form
    {

        SerialPort ser = new SerialPort();
     

        public Form1()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

            cmbPorts.Items.Clear();

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }

            cmbPorts.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }
            cmbPorts.SelectedIndex = 0;

            DateTime now = DateTime.Now;
            txtDatetime.Text = now.ToLongTimeString() + " - ";
            txtDatetime.Text += now.ToShortDateString();
            btnDisconnect.Enabled = false;
            btnSync.Enabled = false;
           
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {

            if (cmbPorts.SelectedItem == null)
            {
                MessageBox.Show("NO SERIAL PORT SELECTED", "ERROR");
            }
            else
            {
                ser.BaudRate = 115200;
                ser.PortName = cmbPorts.SelectedItem.ToString();
                ser.DataBits = 8;
                ser.StopBits = StopBits.One;
                ser.Parity = Parity.None;
                ser.Handshake = Handshake.None;
                ser.ReadTimeout = 5000;
                try
                {
                    ser.Open();

                    if (ser.IsOpen == true)
                    {
                        btnRefresh.Enabled = false;
                        btnDisconnect.Enabled = true;
                        btnConnect.Enabled = false;
                        cmbPorts.Enabled = false;
                        btnSync.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "ERROR");
                }
            }
     
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            txtDatetime.Text = now.ToLongTimeString() + " - ";
            txtDatetime.Text += now.ToShortDateString();

            if(ser.IsOpen == false)
            {
                btnRefresh.Enabled = true;
                btnDisconnect.Enabled = false;
                btnConnect.Enabled = true;
                cmbPorts.Enabled = true;
                btnSync.Enabled = false;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {

            try
            {
                ser.Close();
                if (ser.IsOpen == false)
                {
                    btnConnect.Enabled = true;
                    btnRefresh.Enabled = true;
                    btnDisconnect.Enabled = false;
                    cmbPorts.Enabled = true;
                    btnSync.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "ERROR");
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            btnSync.Enabled = false;
            string response ="";
            if (ser.IsOpen)
            {
                ser.Write("SYNC\n");
                txtLog.Text += "SYNC ";
                
                try
                {
                    response = ser.ReadLine();

                    if (response.CompareTo("WAIT FOR SYNC") == 0)
                    {
                        
                        String data = string.Format("H{0}N{1}S{2}D{3}M{4}Y{5}", now.Hour, now.Minute, now.Second, now.Day, now.Month, now.Year);
                        ser.Write(data + "\n");
                        txtLog.Text += data + "\r\n";
                    }
                  
                }
                catch (Exception ex)
                {
                    txtLog.Text += "SYNC FAILED " + ex.Message + "\r\n";
                }
               

            try {
                    response = ser.ReadLine();

                    if (response.CompareTo("SYNC OK") == 0)
                    {
                        txtLog.Text += "SYNC COMPLETED\r\n";
                    }
                    else
                    {
                      txtLog.Text += "SYNC FAILED\r\n";
                    }
                }
                catch (Exception ex)
                {
                    txtLog.Text += "SYNC FAILED "+ ex.Message + "\r\n";
                }
            }
            btnSync.Enabled = true;

        }

       
    }
}
