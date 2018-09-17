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
            BotManager manager = new BotManager(client);

            client.Connect();

            bool readNext = true;
            while (readNext)
            {
                string input = Console.ReadLine();

                if (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    readNext = false;
                }
            }
        }
    }
}