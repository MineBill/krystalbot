using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrystalBot
{
    class BotManager
    {
        BotClient client;
        public BotManager(BotClient client)
        {
            this.client = client;

            TaskParser parser = new TaskParser(client);
            Task[] tasks = parser.Parse(Environment.CurrentDirectory + "/tasks.cfg");
            foreach (Task task in tasks)
            {
                task.Run();
            }

            client.OnCommandReceived += Client_OnCommandReceived;
        }

        private void Client_OnCommandReceived(string c)
        {
            switch (c)
            {
                case "date":
                    client.SendMessage(DateTime.Now.ToString());
                    break;
                case "update":
                    // GetUptime();
                    break;
                default:
                    break;
            }
        }
    }
}
