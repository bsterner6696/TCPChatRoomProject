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
    public class Server
    {
        UserDictionary userDictionary = new UserDictionary();
        Listener listener = new Listener();
        Writer writer = new Writer();
        Reader reader = new Reader();

        public void SendMessageToAll()
        {

        }
    }
}
