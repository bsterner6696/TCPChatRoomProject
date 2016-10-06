using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace TCPChatRoomProjectClientSide
{
    class Client
    {
        private TcpClient client;
        
        public Client(string IP, int port)
        {
            client = new TcpClient(IP, port);
        }
        private void WriteMessage()
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            string message = Console.ReadLine();
            writer.WriteLine(message);
            writer.Flush();
            writer = null;
        }
        private void ReadMessage()
        {
            StreamReader reader = new StreamReader(client.GetStream());
            string message = "";
            message = reader.ReadLine();
            if (message != "")
            {
                Console.WriteLine(message);
            }
            reader = null;
        }
        private void WriteMessages()
        {
            while (true)
            {
                WriteMessage();
            }
        }

        private void ReadMessages()
        {
            while (true)
            {
                ReadMessage();
            }
        }
        public void RunClient()
        {

            
                Thread writeThread = new Thread(() => WriteMessages());
                writeThread.Start();
                Thread readThread = new Thread(() => ReadMessages());
                readThread.Start();
        }
    }
}
