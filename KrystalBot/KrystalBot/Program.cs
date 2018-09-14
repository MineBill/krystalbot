using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrystalBot
{
    class Program
    {
        static void Main(string[] args)
        {
            BotSettings settings = new BotSettings();

            BotClient client = new BotClient(settings.BotName, settings.OAuthPass,settings.ChannelName);
            client.Connect();

            TaskParser parser = new TaskParser();

            Console.ReadKey();
        }
    }
}