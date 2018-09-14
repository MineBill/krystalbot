using System;

namespace KrystalBot
{
    class Task
    {
        public Task()
        {

        }

        public Task(int i, string v)
        {
            if (i == -1)
            {
                // Unrecognized type error
                Console.WriteLine($"{v} is not a recognized task type. Please check tasks.cfg");
            }
        }

        internal string[] args;
        internal string content;

        public void GetArgsAndContent(string[] _args, string _content)
        {
            args = new string[_args.Length - 1];
            for (int i = 0; i < _args.Length - 1; i++)
            {
                args[i] = _args[i + 1];
            }
            content = _content;
        }

        public virtual void Run()
        {

        }
    }
}
