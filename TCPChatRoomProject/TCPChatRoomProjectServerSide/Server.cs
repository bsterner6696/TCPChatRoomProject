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
                    writer.writer = new StreamWriter(clients[number].GetStream());
                    writer.writer.WriteLine("{0}: {1}", nickName, message);
                    writer.writer.Flush();
                    writer = null;
                }
                catch 
                {
                    string userName = userDictionary.ClientsByNumber[clients[number]];
                    SendSystemMessage(userName + " has left the chat.");
                    userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[clients[number]]);
                    userDictionary.ClientsByNumber.Remove(clients[number]);

                }
            }
        }

        public void SendSystemMessage(string message)
        {
            Chat.Sockets.TcpClient[] clients = new Chat.Sockets.TcpClient[userDictionary.ClientsByName.Count];
            userDictionary.ClientsByName.Values.CopyTo(clients, 0);
            for (int number = 0; number < clients.Length; number++)
            {
                try
                {
                    writer.writer = new StreamWriter(clients[number].GetStream());
                    writer.writer.Write(message);
                    writer.writer.Flush();
                    writer = null;
                }
                catch
                {
                    userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[clients[number]]);
                    userDictionary.ClientsByNumber.Remove(clients[number]);
                }
            }
        }
    }
}
