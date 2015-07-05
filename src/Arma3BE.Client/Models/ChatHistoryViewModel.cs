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

            SelectedServers = serverId.ToString();
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Filter { get; set; }

        public IEnumerable<ChatView> Log
        {
            get
            {
                using (var dc = new Arma3BeClientContext())
                {
                    //var log = dc.ChatLog.Where(x => x.ServerId == _serverId);

                    var log = dc.ChatLog.AsQueryable();

                    if (!string.IsNullOrEmpty(SelectedServers))
                    {
                        var guids = SelectedServers.Split(new[] { ',' }).Select(Guid.Parse).ToArray();
                        if (guids.Length > 0)
                        {
                            log = log.Where(x => guids.Contains(x.ServerId));
                        }
                    }


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

                    return log.OrderBy(x=>x.Date).Select(x=>new ChatView()
                    {
                        Id = x.Id,
                        Date = x.Date,
                        ServerName = x.ServerInfo.Name,
                        Text = x.Text
                    }).ToList();
                }
            }
        }


        private readonly Lazy<IEnumerable<ServerInfo>> _serverList = new Lazy<IEnumerable<ServerInfo>>(() =>
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.OrderBy(x => x.Name).ToList();
            }
        });

        public IEnumerable<ServerInfo> ServerList
        {
            get { return _serverList.Value; }
        }

        public string SelectedServers { get; set; }

        public ICommand FilterCommand { get; set; }
    }

    public class ChatView
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ServerName { get; set; }
        public DateTime Date { get; set; }
    }
}