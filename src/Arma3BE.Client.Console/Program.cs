using Arma3BE.Client.Modules.MainModule.ViewModel;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace arma3beConsole
{
    internal class Program
    {
        private static void Main()
        {
            XmlConfigurator.Configure();
            var log = new Log();
            log.Info("Startup");


            IEnumerable<ServerInfo> servers;


            using (var dc = new ServerInfoRepository())
            {
                servers = dc.GetActiveServerInfo();
            }

            var models = servers.Select(x => OpenServerInfo(x, log)).ToList();


            while (true)
            {
                try
                {
                    var t = Task.Run(() => Run(models));
                    t.Wait();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        private static void Run(IEnumerable<ServerMonitorModel> models)
        {
            var creators =
                models.Select(x => new Func<Thread>(() => new Thread(() => Run(x)) { IsBackground = true })).ToArray();
            var threads = new Thread[creators.Length];

            while (true)
            {
                for (var i = 0; i < threads.Length; i++)
                {
                    var t = threads[i];
                    if (t == null || !t.IsAlive)
                    {
                        t?.Abort();
                        var creator = creators[i];
                        t = creator();
                        t.Start();
                    }

                    Thread.Sleep(5000);
                }

                Thread.Sleep(120000);
            }
        }

        private static void Run(ServerMonitorModel model)
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