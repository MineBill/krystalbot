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
        public void Start()
        {
            string content = File.ReadAllText(Environment.CurrentDirectory + "/tasks.cfg");
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
                    Task type = GetTaskByString(typeAndArgs[0]);
                    type.GetArgsAndContent(typeAndArgs, taskContent);
                    type.Run();
                }
            }
        }

        private Task GetTaskByString(string v)
        {
            switch (v)
            {
                case "repeatingmsg":
                    return new RepeatingMessage();
                default:
                    return new Task(-1,v);
            }
        }
    }
}