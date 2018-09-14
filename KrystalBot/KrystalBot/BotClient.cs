using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace KrystalBot
{
    class BotClient
    {
        enum IRCMessageType
        {
            ChatMessage,
        }
        
        static Timer updateTimer;

        TcpClient tcpClient;
        StreamWriter writer;
        StreamReader reader;

        protected string channelName;
        protected string userName;
        protected string password;

        protected string twitchHostname = "irc.twitch.tv";
        protected int twitchPort = 6667;

        protected string chatMsgID = "PRIVMSG",welcomMsgID = "Welcome, GLHF!";
        protected string chatMsgPrefix;
        protected string commandPrefix;

        Queue<string> quedMessages;
        DateTime lastMessageTime;
        
        public BotClient(string userName, string password, string channelName)
        {
            Initialize(userName, password, channelName, "~");
        }

        public BotClient(string userName,string password, string channelName, string cmdPrefix)
        {
            Initialize(userName, password, channelName, cmdPrefix);
        }

        void Initialize(string userName,string password,string channelName,string cmdPrefix)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"Error, username or passward was empty. Username:[{userName}] Password:[{this.password}]" +
                    $" Get your OAuth key by going to twitchapps.tmi and using the desired twitch account.");
                return;
            }

            this.userName = userName.ToLower();
            this.password = password;
            this.channelName = channelName.ToLower();
            this.chatMsgPrefix = $":{this.userName}!{this.userName}@{this.userName}.tmi.twitch.tv {chatMsgID} #{this.channelName} :";
            this.commandPrefix = cmdPrefix;

            this.quedMessages = new Queue<string>();
            updateTimer = new Timer(OnUpdate, null, 0, 400);
        }

        public void Connect()
        {
            MakeConnection();
        }

        private void MakeConnection()
        {
            Console.WriteLine(DEBUG("Creating a connection..."));
            Connected = false;
            tcpClient = new TcpClient(twitchHostname, twitchPort);

            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream())
            {
                AutoFlush = true
            };

            writer.WriteLine("PASS " + password + Environment.NewLine
                + "NICK " + userName + Environment.NewLine
                + "USER " + userName + " 8 * :" + userName);
            writer.Flush();
            writer.WriteLine("JOIN #terrestrialgames");
            writer.Flush();
            Console.WriteLine(DEBUG("Finished."));
            lastMessageTime = DateTime.Now;
        }
        
        private void OnUpdate(object state)
        {
            if (tcpClient == null)
                return;

            if (!tcpClient.Connected)
            {
                Connect();
            }

            TrySendMessages();
            TryGetMessages();
        }

        private void TrySendMessages()
        {
            TimeSpan diff = DateTime.Now - lastMessageTime;
            //Console.WriteLine(diff.Seconds);
            bool canSend = diff >= TimeSpan.FromSeconds(3);
            if(canSend)
            {
                if(quedMessages.Count > 0)
                {
                    Console.WriteLine(DEBUG("Sending message..."));

                    string message = quedMessages.Dequeue();
                    writer.WriteLine($"{chatMsgPrefix} {message}");
                    writer.Flush();
                    lastMessageTime = DateTime.Now;
                }
            }
        }

        private void TryGetMessages()
        {
            if (tcpClient.Available > 0 || reader.Peek() >= 0)
            {
                var message = reader.ReadLine();
                if (!Connected && message.Contains(welcomMsgID)) Connected = true;
                
                if (!CheckIfChatMessage(message))
                {
                    Console.WriteLine(IRC_DEBUG(message));
                }
            }
        }

        private bool CheckIfChatMessage(string message)
        {
            var iCollIndex = message.IndexOf(":", 1);
            var iBangIndex = message.IndexOf("!", 1);
            if (iCollIndex > 0)
            {
                var preMsgCmd = message.Substring(0, iCollIndex - 1);
                if (preMsgCmd.Contains(chatMsgID))
                {
                    // Received a viewer message
                    var chatMessage = message.Substring(iCollIndex + 1);
                    var messageSender = message.Substring(1, iBangIndex - 1);

                    if(commandPrefix != "~" && chatMessage.StartsWith(commandPrefix))
                    {
                        var command = chatMessage.Substring(commandPrefix.Length);
                        CommandReceived(command);
                    }
                    else
                    {
                        MessageReceived(chatMessage, messageSender);
                    }
                    
                    return true;
                }
            }

            return false;
        }

        public void SendMessage(string _message)
        {
            quedMessages.Enqueue(_message);
        }

        // Events
        public delegate void D_OnMessageReceived(string m,string s);
        public delegate void D_OnCommandReceived(string c);
        public event D_OnMessageReceived OnMessageReceived;
        public event D_OnCommandReceived OnCommandReceived;

        private void MessageReceived(string message,string sender)
        {
            OnMessageReceived?.Invoke(message, sender);
            Console.WriteLine(CHAT_DEBUG(message,sender));
        }

        private void CommandReceived(string command)
        {
            OnCommandReceived?.Invoke(command);
        }

        // Properties
        public bool Connected { get; private set; }

        public string UserName {
            get {
                return userName;
            }
        }

        // Helper methods
        string CHAT_DEBUG(string msg,string sender = null)
        {
            if (sender != null)
                return $"{DateTime.Now}:CHAT MESSAGE: {msg} SENDER: {sender}";
            else
                return $"{DateTime.Now}:CHAT MESSAGE: {msg}";
        }

        string IRC_DEBUG(string msg)
        {
            return $"{DateTime.Now}:IRC MESSAGE: {msg}";
        }

        string DEBUG(string msg)
        {
            return $"{DateTime.Now}:DEBUG MESSAGE: {msg}";
        }
        
    }
}