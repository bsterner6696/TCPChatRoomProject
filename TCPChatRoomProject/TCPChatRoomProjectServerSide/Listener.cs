using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Chat = System.Net;

namespace TCPChatRoomProjectServerSide
{
    public class Listener
    {
        public System.Net.Sockets.TcpListener serverListener = new System.Net.Sockets.TcpListener(IPAddress.Parse("10.2.20.18"), 8000);

    }
}
