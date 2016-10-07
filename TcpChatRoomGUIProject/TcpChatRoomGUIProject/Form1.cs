using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TcpChatRoomGUIProject
{
    public partial class Form1 : Form
    {
        TcpClient client;

        bool isConnected;



        public void Connect()
        {
            client = new TcpClient("10.2.20.27", 8002);
                     
        }
        
        private async Task ReadMessage()
        {
            try
            {
                StreamReader reader = new StreamReader(client.GetStream());
                string message;

                while ((message = await reader.ReadLineAsync()) != null)
                {
                    textBox2.AppendText(message);
                    textBox2.AppendText(Environment.NewLine);
                }
            }
            catch
            {
                isConnected = false;
                string warningText = "Not connected to chat room.  Try hitting connect.  If that doesn't work, try other stuff. /r/n";                
                textBox2.AppendText(warningText);

            }
        }
        
        private void WriteMessage(string text)
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            string message = text;
            writer.WriteLine(message);
            writer.Flush();
            writer = null;
        }
        
        
        public Form1()
        {
            InitializeComponent();
            isConnected = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                string textEntered = textBox1.Text;
                WriteMessage(textEntered);
                textBox1.Clear();
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (isConnected == false)
            {
                isConnected = true;
                Connect();
                textBox1.Clear();
                ReadMessage(); 
            } 

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string textEntered = textBox1.Text;
                    WriteMessage(textEntered);
                    textBox1.Clear();
                }
                catch
                {
                    isConnected = false;
                    string warningText = "Not connected to chat room.  Try hitting connect.  If that doesn't work, try other stuff. \r\n";
                    textBox2.AppendText(warningText);
                }
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
