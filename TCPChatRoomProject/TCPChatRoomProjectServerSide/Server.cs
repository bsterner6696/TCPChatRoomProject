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
        private UserList userList;
        private TcpListener serverListener;
        private Queue<string> incomingMessages;
        private string localIPAddress;
        private int port;       
        public Server()
        {
            localIPAddress = "10.2.20.26";
            port = 8002;
            serverListener = new TcpListener(IPAddress.Parse(localIPAddress), port);
            userList = new UserList();
            incomingMessages = new Queue<string>();
        }
        private string FormatMessageWithNickName(string nickName, string message)
        {
            string textToSend = nickName + ": " + message;
            return textToSend;
        }

        private void SendMessageToAll(string message)
        {
            TcpClient[] users = new TcpClient[userList.ClientsByName.Count];
            userList.ClientsByName.Values.CopyTo(users, 0);
            for (int number = 0; number < users.Length; number++)
            {
                try
                {
                    SendMessage(users[number], message);

                }
                catch
                {
                    userList.ClientsByName.Remove(userList.NamesByClient[users[number]]);
                    userList.NamesByClient.Remove(users[number]);
                }
            }
        }
        private void SendMessage(TcpClient user, string message)
        {
            try
            {
                NetworkStream network = user.GetStream();
                StreamWriter writer = new StreamWriter(user.GetStream());
                writer.WriteLine(message);
                writer.Flush();
                writer = null;
            }
            catch
            {
                string userName = userList.NamesByClient[user];
                SendMessageToAll(userName + " has left the chat.");
                userList.ClientsByName.Remove(userList.NamesByClient[user]);
                userList.NamesByClient.Remove(user);
            }

        }

        private void ConnectClients()
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

        private void RunThroughQueue()
        {
            while (true)
            {
               
                if (incomingMessages.Count != 0)
                {
                    string line = incomingMessages.Dequeue();
                    SendMessageToAll(line);

                }
            }
        }

        public void RunServer()
        {
            
            Thread messageThread = new Thread(RunThroughQueue);
            messageThread.Start();
            ConnectClients();
        }
        private string ReadMessage(TcpClient user)
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
                string userName = userList.NamesByClient[user];
                string leavingMessage = "EXIT";                
                return leavingMessage;
            }
        }
        private void OpenChatRoomForUser(TcpClient user)
        {

            Thread chatThread = new Thread(() => BeginChat(user));
            chatThread.Start();
        }
        private string GetNickName(TcpClient user)
        {

            SendMessage(user, "Enter your name.");
            return ReadMessage(user);
        }

        private void BeginChat(TcpClient user)
        {

            try
            {
                string welcomeMessage = "Welcome to the chat room.";
                SendMessage(user, welcomeMessage);
                string nickName = GetNickName(user);
                while (userList.ClientsByName.ContainsKey(nickName))
                {
                    SendMessage(user, "Enter a different name");
                    nickName = GetNickName(user);
                }
                userList.ClientsByName.Add(nickName, user);
                userList.NamesByClient.Add(user, nickName);
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
                        userList.ClientsByName.Remove(userList.NamesByClient[user]);
                        userList.NamesByClient.Remove(user);
                        SendMessageToAll(nickName + " has left the chat.");
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
