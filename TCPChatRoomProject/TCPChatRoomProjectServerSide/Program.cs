using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatRoomProjectServerSide
{
    class Program
    {
        static void Main(string[] args)
        {
            TextLogger logger = new TextLogger("log.txt", "errorLog.txt");
            Server server = new Server(logger, "10.2.20.27", 8002);
            server.RunServer();
        }
    }
}
