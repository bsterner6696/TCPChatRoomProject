using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPChatRoomProjectServerSide
{
    public class Server
    {
        private TcpListener serverListener;
        private Queue<string> messages;
        private string localIPAddress;
        private int port;
        private ILoggable logger;
        private Dictionary<TcpClient, string> clients;    
        public Server(ILoggable logger, string IP, int port)
        {
            localIPAddress = IP;
            this.port = port;
            messages = new Queue<string>();
            this.logger = logger;
            clients = new Dictionary<TcpClient, string>();
        }
        private string FormatMessage(string nickName, string message)
        {
            string textToSend = nickName + ": " + message;
            return textToSend;
        }

        private void SendMessageToAll(string message)
        {

            foreach (TcpClient user in clients.Keys)
            {
                try
                {
                    SendMessage(user, message);

                }
                catch
                {
                    clients.Remove(user);
                }
            }
            LogMessage(message);
        }
        private void SendMessage(TcpClient user, string message)
        {
            try
            {
                StreamWriter writer = new StreamWriter(user.GetStream());
                writer.WriteLine(message);
                writer.Flush();
                writer = null;
            }
            catch
            {
                string userName = clients[user];
                SendMessageToAll(userName + " has left the chat.");
                clients.Remove(user);
            }
        }

        public void LogMessage(string message)
        {
            logger.AddToLog(message);
            Console.Clear();
            Console.WriteLine(logger.GetLog());
        }

        private void ConnectClients()
        {
            serverListener = new TcpListener(IPAddress.Parse(localIPAddress), port);
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
               
                if (messages.Count != 0)
                {
                    string line = messages.Dequeue();
                    SendMessageToAll(line);

                }
            }
        }

        public void RunServer()
        {
            
            Thread message = new Thread(RunThroughQueue);
            message.Start();
            ConnectClients();
        }
        private string ReadMessage(TcpClient user)
        {
            try
            {
                StreamReader reader = new StreamReader(user.GetStream());
                string message = reader.ReadLine();
                reader = null;

                return message;
            }
            catch
            {
                string userName = clients[user];
                string leavingMessage = "EXIT";                
                return leavingMessage;
            }
        }
        private void OpenChatRoomForUser(TcpClient user)
        {

            Thread chat = new Thread(() => BeginChat(user));
            chat.Start();
        }
        

        private void BeginChat(TcpClient user)
        {

            try
            {
                string welcomeMessage = "Welcome to the chat room.";
                SendMessage(user, welcomeMessage);
                string nickName = GetNickName(user);
                while (clients.ContainsValue(nickName) || nickName == "EXIT" || nickName == "PM")
                {
                    SendMessage(user, "Enter a different name");
                    nickName = GetNickName(user);
                }
                clients.Add(user, nickName);
                messages.Enqueue(nickName + " has joined the room");
                Thread chat = new Thread(() => RunChat(user));
                chat.Start();
            }
            catch(Exception e)
            {
                logger.AddToErrorLog(e.Message);
                Console.WriteLine("Exception encountered and logged.");
            }
        }

        private string GetNickName(TcpClient user)
        {

            SendMessage(user, "Enter your name.");
            return ReadMessage(user);
        }

        private void RunChat(TcpClient user)
        {
            string nickName = clients[user];
            bool isOn = true;
            bool isPrivate = false;
            bool userChosen = false;
            TcpClient targetUser = user;
            string line = "";
            while (isOn)
            {
                line = ReadMessage(user);
                if (line == "EXIT")
                {
                    clients.Remove(user);
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
                else if (line == "PM")
                {
                    SendMessage(user, "Enter desired recipient");
                    isPrivate = true;
                 }
                else if (isPrivate == true && clients.ContainsValue(line))
                {
                    TcpClient target = clients.First(x => x.Value.Contains(line)).Key;
                    userChosen = true;
                    targetUser = target;
                }
                else if (isPrivate == true && line == "CANCEL")
                {
                    isPrivate = false;
                    userChosen = false;
                }
                else if (userChosen == true)
                {
                    SendMessage(targetUser, nickName + " sent: " + line);
                    userChosen = false;
                    isPrivate = false;
                }
                else
                {
                    line = FormatMessage(nickName, line);
                    messages.Enqueue(line);
                }

            }
        }

        

    }
}
