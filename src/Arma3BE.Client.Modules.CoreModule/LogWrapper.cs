using System;
using System.Diagnostics;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BEClient.Common.Logging;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Arma3BE.Client.Modules.CoreModule
{
    public class LogWrapper:ILog
    {
        private readonly ILog _log;
        private readonly IEventAggregator _aggregator;

        public static ILog CreateDecorator(IUnityContainer container)
        {
            return new LogWrapper(new Log(), container.Resolve<IEventAggregator>());
        }

        public void Debug(object message)
        {
            DebugConditional(message);
        }

        [Conditional("DEBUG")]
        private void DebugConditional(object message)
        {
            _log.Debug(message);

            _aggregator.GetEvent<LoggerMessageEvent>()
                .Publish(new LoggerMessage(LoggerMessage.LogLevel.Debug, message?.ToString()));
        }

        public LogWrapper(ILog log, IEventAggregator aggregator)
        {
            _log = log;
            _aggregator = aggregator;
        }

        public void Info(object message)
        {
            _log.Info(message);

            _aggregator.GetEvent<LoggerMessageEvent>()
                .Publish(new LoggerMessage(LoggerMessage.LogLevel.Info, message?.ToString()));
        }

        public void Error(object message)
        {
            _log.Error(message);

            _aggregator.GetEvent<LoggerMessageEvent>()
                .Publish(new LoggerMessage(LoggerMessage.LogLevel.Error, message?.ToString(), message as Exception));
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);

            _aggregator.GetEvent<LoggerMessageEvent>()
                .Publish(new LoggerMessage(LoggerMessage.LogLevel.Fatal, message?.ToString(), message as Exception));
        }
    }
}