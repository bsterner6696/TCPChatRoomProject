﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace TCPChatRoomProjectServerSide
{
    public class UserList
    {
        public Dictionary<string, TcpClient> ClientsByName = new Dictionary<string, TcpClient>();

        public Dictionary<TcpClient, string> NamesByClient = new Dictionary<TcpClient, string>(); 
    }
}