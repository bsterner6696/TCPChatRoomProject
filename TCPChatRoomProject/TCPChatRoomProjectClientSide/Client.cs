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
        private string serverIP;
        private int port;
        private TcpClient client;
        
        public Client()
        {
            serverIP = "10.2.20.26";
            port = 8002;
            client = new TcpClient(serverIP, port);
        }
        private void WriteMessage()
        {
            NetworkStream network = client.GetStream();
            StreamWriter writer = new StreamWriter(client.GetStream());
            string message = Console.ReadLine();
            writer.WriteLine(message);
            writer.Flush();
            writer = null;
        }
        private void ReadMessage()
        {
            NetworkStream network = client.GetStream();
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
