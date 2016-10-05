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
        TcpListener serverListener = new TcpListener(IPAddress.Parse("10.2.20.26"), 8002);
        public Queue<string> incomingMessages = new Queue<string>();

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

        public string FormatMessageWithNickName(string nickName, string message)
        {
            string textToSend = nickName + ": " + message;
            return textToSend;
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
                    serverListener.Start();

                    TcpClient user = serverListener.AcceptTcpClient();
                    OpenChatRoomForUser(user);
                }
                catch
                {
                    continue;
                   
                }
            }
        }

        public void RunThroughQueue()
        {
            while (true)
            {
               
                if (incomingMessages.Count != 0)
                {
                    string line = incomingMessages.Dequeue();
                    SendSystemMessageToAll(line);

                }
            }
        }

        public void RunServer()
        {
            
            Thread messageThread = new Thread(RunThroughQueue);
            messageThread.Start();
            ConnectClients();
        }
        public string ReadMessage(TcpClient user)
        {
            try
            {
                NetworkStream network = user.GetStream();
                StreamReader reader = new StreamReader(user.GetStream());
                string message = reader.ReadLine();
                reader = null;

                return message;
            }
            catch
            {
                string userName = userDictionary.ClientsByNumber[user];
                string leavingMessage = "EXIT";                
                return leavingMessage;
            }
        }
        public void OpenChatRoomForUser(TcpClient user)
        {

            Thread chatThread = new Thread(() => EnterChat(user));
            chatThread.Start();
        }
        private string GetNickName(TcpClient user)
        {

            SendSystemMessage(user, "Enter your name.");

            return ReadMessage(user);
        }

        public void EnterChat(TcpClient user)
        {

            try
            {
                SendSystemMessage(user, "You're a real bugel boy.");
                string nickName = GetNickName(user);
                while (userDictionary.ClientsByName.ContainsKey(nickName))
                {
                    SendSystemMessage(user, "Enter a different name");
                    nickName = GetNickName(user);
                }
                userDictionary.ClientsByName.Add(nickName, user);
                userDictionary.ClientsByNumber.Add(user, nickName);
                incomingMessages.Enqueue(nickName + " has joined the room");
                Thread chatThread = new Thread(() => RunChat(nickName, user));
                chatThread.Start();
            }
            catch
            {

            }


        }

        private void RunChat(string nickName, TcpClient user)
        {

            bool isOn = true;
                string line = "";
                while (isOn)
                {
                    line = ReadMessage(user);
                    if (line == "EXIT")
                    {
                        userDictionary.ClientsByName.Remove(userDictionary.ClientsByNumber[user]);
                        userDictionary.ClientsByNumber.Remove(user);
                        SendSystemMessageToAll(nickName + " has left the chat.");
                    isOn = false;
                        try
                        {
                            user.Close();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        line = FormatMessageWithNickName(nickName, line);
                        incomingMessages.Enqueue(line);
                    }

                }

        }

    }
}
