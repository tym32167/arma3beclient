using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
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


            var servers = new List<ServerInfo>();
            

            using (var dc = new Arma3BeClientContext())
            {
                servers = dc.ServerInfo.Where(x => x.Active).ToList();
            }

            var models = servers.Select(x=>OpenServerInfo(x, log)).ToList();
            

            while (true)
            {
                try
                {
                    var t = Task.Run(() => run(models));
                    t.Wait();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
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
            var model = new ServerMonitorModel(obj, log, true);

            model.ChatViewModel.ChatMessageEventHandler += (s, e) =>
            {
                Console.WriteLine(@"{0} {1:t}:{2}", model.CurrentServer.Name, e.Message.Date.ToLocalTime(), e.Message);
                Console.WriteLine();
            };

            return model;
        }
    }
}
