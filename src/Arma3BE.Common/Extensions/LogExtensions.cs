using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Arma3BEClient.Common.Logging;

namespace Arma3BEClient.Common.Extensions
{
    public static class LogExtensions
    {
        public static IDisposable Time(this ILog log, string message = "", [CallerMemberName] string member = "",
            [CallerFilePath] string file = "", [CallerLineNumber] int lineNumber = 0)
        {
            var sw = new Stopwatch();
            sw.Start();
            return new DisposableAction(() =>
            {
                sw.Stop();
                log.Info($"TIME: {sw.Elapsed} FOR {file}:{lineNumber} {member} {message}");
            });
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}