using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using Chat = System.Net;

namespace TCPChatRoomProjectClientSide
{
    class Client
    {
        System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient("10.2.20.18", 8000);
        StreamReader reader;
        StreamWriter writer;

        public void ConnectToServer()
        {
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());

        }

        public void WriteMessage()
        {
            string line;
            line = Console.ReadLine();
            writer.WriteLine(line);
        }
        public void RunClient()
        {
            ConnectToServer();
            while (true)
            {
                WriteMessage();
            }
        }
    }
}
