using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BEClient.Libs.Repositories;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public sealed class BELogic : IBELogic
    {
        public BELogic(IEventAggregator aggregator)
        {
            aggregator.GetEvent<BEMessageEvent<BECommand>>().Subscribe(ProcessCommand, ThreadOption.BackgroundThread);
            aggregator.GetEvent<BEMessageEvent<BEPlayerLogMessage>>().Subscribe(BEPlayerLogMessage, ThreadOption.BackgroundThread);
            aggregator.GetEvent<BEMessageEvent<BEBanLogMessage>>().Subscribe(BEBanLogMessage, ThreadOption.BackgroundThread);
            aggregator.GetEvent<BEMessageEvent<BEAdminLogMessage>>().Subscribe(BEAdminLogMessage, ThreadOption.BackgroundThread);
            aggregator.GetEvent<ConnectServerEvent>().Subscribe(BeServerConnectHandler, ThreadOption.BackgroundThread);
        }

        public event EventHandler<ServerCommandEventArgs> ServerUpdateHandler;
        public event EventHandler<ServerCommandEventArgs> ServerImmediateUpdateHandler;

        private void ProcessCommand(BECommand command)
        {
            if (command.CommandType == CommandType.RemoveBan || command.CommandType == CommandType.AddBan ||
                command.CommandType == CommandType.Ban)
            {
                Task.Delay(2000).ContinueWith(t =>
                {
                    OnServerUpdateHandler(new BECommand(command.ServerId, CommandType.Bans));
                });
            }
        }

        private void BeServerConnectHandler(ServerInfoDto info)
        {
            OnServerImmediateUpdateHandler(new BECommand(info.Id, CommandType.Players));

            OnServerUpdateHandler(new BECommand(info.Id, CommandType.Bans));
            OnServerUpdateHandler(new BECommand(info.Id, CommandType.Missions));
            OnServerUpdateHandler(new BECommand(info.Id, CommandType.Admins));
        }

        private void BEPlayerLogMessage(BEPlayerLogMessage message)
        {
            OnServerUpdateHandler(new BECommand(message.ServerId, CommandType.Players));
        }

        private void BEAdminLogMessage(BEAdminLogMessage message)
        {
            OnServerUpdateHandler(new BECommand(message.ServerId, CommandType.Admins));
        }

        private void BEBanLogMessage(BEBanLogMessage message)
        {
            OnServerUpdateHandler(new BECommand(message.ServerId, CommandType.Players));
            OnServerUpdateHandler(new BECommand(message.ServerId, CommandType.Bans));
        }

        private void OnServerUpdateHandler(BECommand command)
        {
            ServerUpdateHandler?.Invoke(this, new ServerCommandEventArgs(command));
        }

        private void OnServerImmediateUpdateHandler(BECommand command)
        {
            ServerImmediateUpdateHandler?.Invoke(this, new ServerCommandEventArgs(command));
        }
    }
}