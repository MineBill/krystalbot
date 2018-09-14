using System;
using System.Threading;

namespace KrystalBot
{
    class RepeatingMessage : Task
    {
        static Timer timer;

        public override void Run()
        {
            timer = new Timer(OnTimerCallback, null, 0, int.Parse(args[0]) * 1000);
        }

        private void OnTimerCallback(object state)
        {
            if(Client.Connected)
                Client.SendMessage(content);
        }
    }
}