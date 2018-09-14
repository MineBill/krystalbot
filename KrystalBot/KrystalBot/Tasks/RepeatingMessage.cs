using System;
using System.Threading;

namespace KrystalBot
{
    class RepeatingMessage : Task
    {
        static Timer timer;
        BotClient client;
        public override void Run()
        {
            client = BotClient.Instance;
            timer = new Timer(OnTimerCallback, null, 0, int.Parse(args[0]) * 1000);
        }

        private void OnTimerCallback(object state)
        {
            client.SendMessage(content);
        }
    }
}
