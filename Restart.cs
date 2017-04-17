using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using UnityEngine;
using Rocket.Core.Plugins;
using Rocket.Core.Commands;
using SDG.Unturned;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NightFish.Restart
{
    public class Restart : RocketPlugin<RestartConfiguration>
    {
        bool Restarting = false;
        string cl = "";
        int PID = 0;

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        protected override void Load()
        {
            if(!File.Exists("Plugins\\Restart\\RestartDaemon.exe"))
            {
                using (Stream input = Assembly.GetManifestResourceStream("NightFish.Restart.RestartDaemon.exe"))
                using (Stream output = File.Create("Plugins\\Restart\\RestartDaemon.exe"))
                {
                    CopyStream(input, output);
                }
            }
            PID = Process.GetCurrentProcess().Id;
            string[] args = Environment.GetCommandLineArgs();
            args[0] = "\"" + args[0] + "\"";
            foreach(string arg in args)
            {
                cl += arg + " ";
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            if (Configuration.Instance.RestartOnShutdown)
                Console.WriteLine("NightFish.Restart> Auto restart on shutdown: Enabled!");
            else
                Console.WriteLine("NightFish.Restart> Auto restart on shutdown: Disabled!");
        }

        protected override void Unload()
        {
            if(Restarting || Configuration.Instance.RestartOnShutdown)
            {
                if (!File.Exists("Plugins\\Restart\\RestartDaemon.exe"))
                {
                    using (Stream input = Assembly.GetManifestResourceStream("NightFish.Restart.RestartDaemon.exe"))
                    using (Stream output = File.Create("Plugins\\Restart\\RestartDaemon.exe"))
                    {
                        CopyStream(input, output);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("NightFish.Restart> Restart triggered!");
                Process.Start("Plugins\\Restart\\RestartDaemon.exe", cl + PID);
            }
        }

        [RocketCommandAlias("reboot")]
        [RocketCommand("restart", "Restarts the Unturned server!", "/restart", AllowedCaller.Both)]
        public void executeCommandRestart(IRocketPlayer caller, string[] args)
        {
            Restarting = true;
            Provider.shutdown(); 
        }
    }
}
