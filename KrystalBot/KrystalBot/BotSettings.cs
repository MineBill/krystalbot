using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrystalBot
{
    class BotSettings
    {
        public BotSettings()
        {
            string[] cfgLines = File.ReadAllLines(Environment.CurrentDirectory + "/config.cfg");

            string[] valueNames = new string[cfgLines.Length];
            string[] values = new string[cfgLines.Length];
            for (int i = 0; i < cfgLines.Length; i++)
            {
                var line = cfgLines[i];
                var nameCleared = line.Replace(" ", string.Empty);
                var valueName = nameCleared.Substring(0, nameCleared.IndexOf('=', 0));
                var value = nameCleared.Substring(nameCleared.IndexOf('=', 0) + 1);

                valueNames[i] = valueName;
                values[i] = value;
            }

            botname = values[Array.IndexOf(valueNames, "botname")];
            oauthpass = values[Array.IndexOf(valueNames, "oauthtoken")];
            channelname = values[Array.IndexOf(valueNames, "channeltoconnect")];
        }

        readonly string botname,oauthpass,channelname;

        public string BotName {
            get {
                return botname;
            }
        }

        public string OAuthPass {
            get {
                return oauthpass;
            }
        }

        public string ChannelName {
            get {
                return channelname;
            }
        }
    }
}
