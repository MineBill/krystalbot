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

        public static BotClient Instance;
        static Timer updateTimer;

        TcpClient tcpClient;
        StreamWriter writer;
        StreamReader reader;

        readonly string channelName;
        readonly string userName;
        readonly string password;

        readonly string twitchHostname = "irc.twitch.tv";
        readonly int twitchPort = 6667;
        
        readonly string chatMsgID = "PRIVMSG";
        readonly string chatMsgPrefix;

        Queue<string> quedMessages;
        DateTime lastMessageTime;

        public string UserName {
            get {
                return userName;
            }
        }

        public BotClient(string userName, string password, string channelName)
        {
            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"Error, username or passward was empty. Username:[{userName}] Password:[{password}]" +
                    $"Get you OAuth key by going to twitchapps.tmi and using the desired twitch account.");
                return;
            }

            Instance = this;

            this.userName = userName.ToLower();
            this.password = password;
            this.channelName = channelName.ToLower();

            chatMsgPrefix = $":{this.userName}!{this.userName}@{this.userName}.tmi.twitch.tv {chatMsgID} #{this.channelName} :";
            
            updateTimer = new Timer(OnUpdate, null, 0, 400);
            quedMessages = new Queue<string>();
        }

        internal void Connect()
        {
            MakeConnection();
        }

        private void MakeConnection()
        {
            Console.WriteLine("Creating a connection...");
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
            Console.WriteLine("Connection established.");
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
                    Console.WriteLine("Sending a message...");

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
                bool wasChatMsg = false;
                var message = reader.ReadLine();
                wasChatMsg = CheckIfChatMessage(wasChatMsg, message);

                if (!wasChatMsg)
                {
                    Console.WriteLine(message);
                }
            }
        }

        private bool CheckIfChatMessage(bool wasChatMsg, string message)
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
                    var msgSender = message.Substring(1, iBangIndex - 1);
                    OnMessageReceived(chatMessage, msgSender);

                    wasChatMsg = true;
                }
            }

            return wasChatMsg;
        }

        public void SendMessage(string _message)
        {
            quedMessages.Enqueue(_message);
        }

        private void OnMessageReceived(string message,string sender)
        {
            if (message.StartsWith("hi", StringComparison.InvariantCultureIgnoreCase))
            {
                SendMessage($"Hello to you too, {sender}");
            }
            Console.WriteLine("VIEWER MESSAGE: " + message + " SENDER: " + sender);
        }
    }
}
