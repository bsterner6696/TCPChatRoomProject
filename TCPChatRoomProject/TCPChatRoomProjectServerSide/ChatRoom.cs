using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using Chat = System.Net;


namespace TCPChatRoomProjectServerSide
{
    public class ChatRoom
    {
        Reader reader = new Reader();
        Writer writer = new Writer();
        System.Net.Sockets.TcpClient client;

        public ChatRoom(System.Net.Sockets.TcpClient tcpClient, UserDictionary dictionary, Server server)
        {
            client = tcpClient;

            Thread chatThread = new Thread(() => EnterChat(client, dictionary, server));
            chatThread.Start();
        }
        private string GetNickName(System.Net.Sockets.TcpClient client)
        {
            writer.writer = new StreamWriter(client.GetStream());
            reader.reader = new StreamReader(client.GetStream());
            writer.writer.WriteLine("What's your name? ");
            writer.writer.Flush();
            return reader.reader.ReadLine();
        }

        public void EnterChat(System.Net.Sockets.TcpClient client, UserDictionary dictionary, Server server)
        {
            reader.reader = new StreamReader(client.GetStream());
            writer.writer = new StreamWriter(client.GetStream());
            writer.writer.WriteLine("You're a real bugel boy.");
            string nickName = GetNickName(client);
            while (dictionary.ClientsByName.ContainsKey(nickName))
            {
                writer.writer.WriteLine("Pick a different name.");
                nickName = GetNickName(client);
            }
            dictionary.ClientsByName.Add(nickName, client);
            dictionary.ClientsByNumber.Add(client, nickName);
            server.SendSystemMessageToAll(nickName + " has joined the room");
            Thread chatThread = new Thread(() => RunChat(server, nickName));
            chatThread.Start();


        }

        private void RunChat(Server server, string nickName)
        {
            string line = "";
            while (true)
            {
                line = reader.reader.ReadLine();
                server.SendMessageToAll(nickName, line);
            }
        }
    }
}
