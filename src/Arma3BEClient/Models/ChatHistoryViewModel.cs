using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public class ChatHistoryViewModel:ViewModelBase
    {
        private readonly Guid _serverId;

        public ChatHistoryViewModel(Guid serverId)
        {
            _serverId = serverId;

            FilterCommand = new ActionCommand(() => RaisePropertyChanged("Log"));
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Filter { get; set; }

        public IEnumerable<ChatLog> Log
        {
            get
            {
                using (var dc = new Arma3BeClientContext())
                {
                    var log = dc.ChatLog.Where(x => x.ServerId == _serverId);
                    if (StartDate.HasValue)
                    {
                        var date = StartDate.Value;
                        log = log.Where(x => x.Date >= date);
                    }

                    if (EndDate.HasValue)
                    {
                        var date = EndDate.Value;
                        log = log.Where(x => x.Date <= date);
                    }

                    if (!string.IsNullOrEmpty(Filter))
                    {
                        log = log.Where(x => x.Text.Contains(Filter));
                    }

                    return log.OrderBy(x=>x.Date).ToList();
                }
            }
        }

        public ICommand FilterCommand { get; set; }
    }
}