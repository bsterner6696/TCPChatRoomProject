using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace TCPChatRoomProjectServerSide
{
    public class Listener
    {
        public TcpListener serverListener = new TcpListener(IPAddress.Any, 8002);

    }
}
