using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Lib.Context;
using Arma3BEClient.Lib.ModelCompact;
using Arma3BEClient.Models;
using Arma3BEClient.ViewModel;

namespace arma3beConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var log = new Log();
            log.Info("Startup");

            List<ServerInfo> servers = new List<ServerInfo>();
            

            using (var dc = new Arma3BeClientContext())
            {
                servers = dc.ServerInfo.Where(x => x.Active).ToList();
            }

            var models = servers.Select(x=>OpenServerInfo(x, log)).ToList();

            //while (true)
            //{
            //    foreach (var model in models)
            //    {
            //        if (!model.Connected)
            //        {
            //            model.Connect();
            //            Console.WriteLine(@"Connectiong to {0}", model.CurrentServer.Name);
            //            Thread.Sleep(2000);
            //        }
            //    }

            //    Thread.Sleep(30000);
            //}


            var t = new Thread(() => run(models)) { IsBackground = true };
            t.Start();

            while (true)
            {
                Thread.Sleep(1000);
                if (!t.IsAlive)
                {
                    try
                    {
                        t.Abort();
                        t = new Thread(() => run(models)) { IsBackground = true };
                        t.Start();
                    }
                    catch
                    {
                    }
                }
            }
            
            models.ForEach(x=>x.Cleanup());
        }

        private static void ThreadKeeper(IEnumerable<ServerMonitorModel> models)
        {
            /*var ts = new ParameterizedThreadStart(run);

            var thread = new Thread(ts);
            thread.Start(models);

            while (true)
            {
                Thread.Sleep(1000);

            }*/
        }

        private static void run(IEnumerable<ServerMonitorModel> models)
        {
            var creators = models.Select(x => new Func<Thread>(() => new Thread(() => run(x)){IsBackground = true})).ToArray();
            var threads = new Thread[creators.Length];

            while (true)
            {
                for (var i = 0; i < threads.Length; i++)
                {
                    var t = threads[i];
                    if (t == null || !t.IsAlive)
                    {
                        if (t != null) t.Abort();
                        var creator = creators[i];
                        t = creator();
                        t.Start();
                    }

                    Thread.Sleep(5000);
                }

                Thread.Sleep(120000);
            }
        }

        private static void run(ServerMonitorModel model)
        {
            while (true)
            {
                if (!model.Connected)
                {
                    model.Connect();
                    Console.WriteLine(@"Connectiong to {0}", model.CurrentServer.Name);
                }
                Thread.Sleep(123000);
            }
        }

        private static ServerMonitorModel OpenServerInfo(ServerInfo obj, ILog log)
        {
            var model = new ServerMonitorModel(obj, log);

            model.ChatViewModel.ChatMessageEventHandler += (s, e) =>
            {
                Console.WriteLine(@"{0} {1:t}:{2}", model.CurrentServer.Name, e.Date.ToLocalTime(), e.Message);
                Console.WriteLine();
            };

            return model;
        }
    }
}
