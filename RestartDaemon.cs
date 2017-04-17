using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace RestartDaemon
{
    class RestartDaemon
    {
        static void Main(string[] args)
        {
            try
            {
                int retries = 0;
                Process proc = null;
                bool initRun = true;
                string path = args[0];
                string parameters = "";
                int PID = Convert.ToInt32(args[args.Length - 1]);
                for(int i = 1; i < args.Length - 1; i++)
                {
                    parameters += " " + args[i];
                }
                parameters = parameters.Substring(1);
                Console.Title = "NightFish Auto Restart";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Waiting for server to shutdown...");
                while(true)
                {
                    try
                    {
                        if (initRun)
                        { 
                            if (Process.GetProcessById(PID) == null)
                                initRun = false;
                            else
                                retries++;
                        }
                    }
                    catch
                    {
                        initRun = false;
                    }

                    if(!initRun)
                    {
                        Console.WriteLine("Starting Unturned...");
                        ProcessStartInfo psi = new ProcessStartInfo();
                        path = path.Replace("/", "\\");
                        psi.FileName = path;
                        psi.Arguments = parameters;
                        psi.WorkingDirectory = path.Substring(0, path.LastIndexOf("\\"));
                        proc = Process.Start(psi);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Verifying restart...");
                        Thread.Sleep(10000);
                        
                        if (proc != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Restart successful!");
                            Thread.Sleep(1000);
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed! Retrying...");
                            retries++;
                        }
                    }

                    if(retries >= 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed after 10 retries!");
                        Thread.Sleep(5000);
                        break;
                    }
                    Thread.Sleep(1000);

                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                Thread.Sleep(5000);
            }
        }
    }
}
