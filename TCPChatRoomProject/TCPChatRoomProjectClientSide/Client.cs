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
        TcpClient client = new TcpClient("10.2.20.26", 8002);
        
        public void WriteMessage()
        {
            NetworkStream network = client.GetStream();
            StreamWriter writer = new StreamWriter(client.GetStream());
            string message = Console.ReadLine();
            writer.WriteLine(message);
            writer.Flush();
            writer = null;
        }
        public void ReadMessage()
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
        public void WriteMessages()
        {
            while (true)
            {
                WriteMessage();
            }
        }

        public void ReadMessages()
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
