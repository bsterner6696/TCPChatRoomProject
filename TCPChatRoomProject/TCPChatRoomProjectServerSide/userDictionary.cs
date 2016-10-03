using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using Chat = System.Net;


namespace TCPChatRoomProjectServerSide
{
    public class userDictionary
    {
        public Dictionary<string, Chat.Sockets.TcpClient> ClientsByName = new Dictionary<string, Chat.Sockets.TcpClient>();

        public Dictionary<Chat.Sockets.TcpClient, string> ClientsByNumber = new Dictionary<Chat.Sockets.TcpClient, string>(); 
    }
}
