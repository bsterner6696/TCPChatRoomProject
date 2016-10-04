using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace TCPChatRoomProjectServerSide
{
    public class Server
    {
        UserDictionary userDictionary = new UserDictionary();
        Listener listener = new Listener();

        public void SendMessageToAll(string nickName, string message)
        {
            TcpClient[] clients = new TcpClient[userDictionary.ClientsByName.Count];
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

        public void SendMessage(TcpClient client, string nickName, string message)
        {
            string textToSend = nickName + ": " +  message;
            NetworkStream network = client.GetStream();
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(textToSend);
            writer.Flush();
            writer = null;

        }

        public void SendSystemMessageToAll(string message)
        {
            TcpClient[] clients = new TcpClient[userDictionary.ClientsByName.Count];
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
        public void SendSystemMessage(TcpClient client, string message)
        {
            try
            {
                NetworkStream network = client.GetStream();
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.WriteLine(message);
                writer.Flush();
                writer = null;
            }
            catch
            {
                string userName = userDictionary.ClientsByNumber[client];
                SendSystemMessageToAll(userName + " has left the chat.");
                userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[client]);
                userDictionary.ClientsByNumber.Remove(client);
            }

        }

        public void ConnectClients()
        {
            while (true)
            {
                try
                {
                    listener.serverListener.Start();

                    TcpClient connectedClient = listener.serverListener.AcceptTcpClient();
                    ChatRoom chatRoom = new ChatRoom(connectedClient, userDictionary, this);
                }
                catch
                {
                    continue;
                   
                }
            }
        }

        public string ReadMessage(TcpClient tcpclient)
        {
            try
            {
                TcpClient client = tcpclient;
                NetworkStream network = client.GetStream();
                StreamReader reader = new StreamReader(client.GetStream());
                string message = reader.ReadLine();
                reader = null;

                return message;
            }
            catch
            {
                string userName = userDictionary.ClientsByNumber[tcpclient];
                string leavingMessage = userName + " has left the chat.";
                userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[tcpclient]);
                userDictionary.ClientsByNumber.Remove(tcpclient);
                return leavingMessage;
            }
        }

        public static string ProcessClientRequest(TcpClient tcpclient)
        {
            
            TcpClient client = tcpclient;
            NetworkStream network = client.GetStream();
            StreamReader reader = new StreamReader(client.GetStream());
            string message = reader.ReadLine();
            reader = null;

            return message;
            
            
        }
    }
}
