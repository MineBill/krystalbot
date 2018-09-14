using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KrystalBot
{
    class TaskParser
    {
        BotClient client;

        public TaskParser(BotClient client)
        {
            this.client = client;
        }

        public Task[] Parse(string path)
        {
            List<Task> tasks = new List<Task>();

            string content = File.ReadAllText(path);
            content = content.Replace(Environment.NewLine, " ");
            //Console.WriteLine(lines);
            if (content.Length > 0)
            {
                bool parsing = true;
                while (parsing)
                {
                    int start, end;
                    string wholeTask, taskTypeAndArgs, taskContent;

                    start = content.IndexOf("#", 0);
                    end = content.IndexOf("#", start + 1);

                    wholeTask = content.Substring(start + 1, end - 1);
                    wholeTask = wholeTask.Remove(0, 1);

                    taskTypeAndArgs = wholeTask.Substring(0, wholeTask.IndexOf(" "));
                    taskContent = wholeTask.Substring(wholeTask.IndexOf(" ", 0));

                    string[] typeAndArgs = taskTypeAndArgs.Split(new char[] { '-' });
                    content = content.Substring(end + 1);
                    if (content == "" || content == " ")
                    {
                        parsing = false;
                    }

                    //Console.WriteLine(taskContent);
                    Task task = GetTaskByString(typeAndArgs[0]);
                    task.GetArgsAndContent(typeAndArgs, taskContent);

                    tasks.Add(task);
                }
            }

            return tasks.ToArray();
        }

        private Task GetTaskByString(string v)
        {
            switch (v)
            {
                case "repeatingmsg":
                    return new RepeatingMessage() { Client = client};
                default:
                    return new Task(-1,v);
            }
        }
    }
}