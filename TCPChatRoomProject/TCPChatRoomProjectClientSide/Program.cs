using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatRoomProjectClientSide
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("10.2.20.26", 8002);
            client.RunClient();
        }
    }
}
