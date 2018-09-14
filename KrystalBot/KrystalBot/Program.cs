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
            BotClient client = new BotClient(settings.BotName, settings.OAuthPass,settings.ChannelName,"!");
            client.Connect();

            BotManager manager = new BotManager(client);
            
            Console.ReadKey();
        }
    }
}