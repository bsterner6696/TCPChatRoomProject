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

        public void SendMessageToAll(string nickName, string message)
        {
            Chat.Sockets.TcpClient[] clients = new Chat.Sockets.TcpClient[userDictionary.ClientsByName.Count];
            userDictionary.ClientsByName.Values.CopyTo(clients, 0);
            for (int number = 0; number < clients.Length; number++)
            {
                try
                {
                    SendMessage(clients[number], nickName, message);
                }
                catch
                {
                    string userName = userDictionary.ClientsByNumber[clients[number]];
                    SendSystemMessageToAll(userName + " has left the chat.");
                    userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[clients[number]]);
                    userDictionary.ClientsByNumber.Remove(clients[number]);

                }
            }
        }

        public void SendMessage(Chat.Sockets.TcpClient client, string nickName, string message)
        {
            writer.writer = new StreamWriter(client.GetStream());
            writer.writer.WriteLine("{0}: {1}", nickName, message);
            writer.writer.Flush();
            writer = null;
        }

        public void SendSystemMessageToAll(string message)
        {
            Chat.Sockets.TcpClient[] clients = new Chat.Sockets.TcpClient[userDictionary.ClientsByName.Count];
            userDictionary.ClientsByName.Values.CopyTo(clients, 0);
            for (int number = 0; number < clients.Length; number++)
            {
                try
                {
                    SendSystemMessage(clients[number], message);

                }
                catch
                {
                    userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[clients[number]]);
                    userDictionary.ClientsByNumber.Remove(clients[number]);
                }
            }
        }
        public void SendSystemMessage(Chat.Sockets.TcpClient client, string message)
        {
            writer.writer = new StreamWriter(client.GetStream());
            writer.writer.Write(message);
            writer.writer.Flush();
            writer = null;
        }

        public void ConnectClients()
        {
            while (true)
            {
                listener.serverListener.Start();
                if (listener.serverListener.Pending())
                {
                    Chat.Sockets.TcpClient connectedClient = listener.serverListener.AcceptTcpClient();
                    ChatRoom chatRoom = new ChatRoom(connectedClient, userDictionary, this);
                }
            }
        }
    }
}
