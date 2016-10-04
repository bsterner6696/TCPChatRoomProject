using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;


namespace TCPChatRoomProjectServerSide
{
    public class ChatRoom
    {

        TcpClient client = new TcpClient();

        public ChatRoom(TcpClient tcpClient, UserDictionary dictionary, Server server)
        {
            client = tcpClient;

            Thread chatThread = new Thread(() => EnterChat(client, dictionary, server));
            chatThread.Start();
        }
        private string GetNickName(TcpClient client, Server server)
        {

            server.SendSystemMessage(client, "Enter your name.");

            return server.ReadMessage(client);
        }

        public void EnterChat(TcpClient client, UserDictionary dictionary, Server server)
        {
            
            server.SendSystemMessage(client, "You're a real bugel boy.");
            Thread.Sleep(1);
            string nickName = GetNickName(client, server);
            while (dictionary.ClientsByName.ContainsKey(nickName))
            {
                server.SendSystemMessage(client, "Enter a different name");
                nickName = GetNickName(client, server);
            }
            dictionary.ClientsByName.Add(nickName, client);
            dictionary.ClientsByNumber.Add(client, nickName);
            server.SendSystemMessageToAll(nickName + " has joined the room");
            Thread chatThread = new Thread(() => RunChat(server, nickName, client));
            chatThread.Start();


        }

        private void RunChat(Server server, string nickName, TcpClient client)
        {
            try
            {
                string line = "";
                while (true)
                {
                    line = server.ReadMessage(client);
                    server.SendMessageToAll(nickName, line);
                }
            }
            catch (Exception e44)
            {
                Console.WriteLine(e44);
            }
        }
    }
}
