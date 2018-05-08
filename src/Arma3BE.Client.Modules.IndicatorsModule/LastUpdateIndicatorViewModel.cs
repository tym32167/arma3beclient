using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Infrastructure.Models;
using Prism.Events;
using System;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Repositories;

namespace Arma3BE.Client.Modules.IndicatorsModule
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LastUpdateIndicatorViewModel : ViewModelBase
    {
        private readonly ServerInfoDto _serverInfo;

        public LastUpdateIndicatorViewModel(IEventAggregator eventAggregator, ServerInfoDto serverInfo)
        {
            _serverInfo = serverInfo;
            eventAggregator.GetEvent<BEMessageEvent<BEMessage>>().Subscribe(AddMessage);
        }

        private void AddMessage(BEMessage message)
        {
            if (_serverInfo.Id != message.ServerId) return;
            LastUpdate = $"Last update at {DateTime.UtcNow.UtcToLocalFromSettings().ToLongTimeString()}";
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(LastUpdate));
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string LastUpdate { get; private set; }
    }
}