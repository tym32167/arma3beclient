using Arma3BE.Client.Modules.ToolsModule.Virtual;
using Arma3BE.Server.Abstract;
using BattleNET;
using System;
using System.Windows;

namespace Arma3BE.Client.Modules.ToolsModule
{
    /// <summary>
    /// Interaction logic for VirtualServerWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class VirtualServerWindow : Window
    {
        private readonly VirtualServer _server;

        public VirtualServerWindow(IBattlEyeServerFactory factory)
        {
            InitializeComponent();

            _server = factory.Create(new BattlEyeLoginCredentials()) as VirtualServer;
            _server.CommandRecieved += _server_CommandRecieved;
            _server.PureCommandRecieved += _server_PureCommandRecieved;
        }

        private void _server_PureCommandRecieved(object sender, VirtualServer.PureCommandArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                tbCommand.Text += $"{e.Command}" + Environment.NewLine;
            });
        }

        private void _server_CommandRecieved(object sender, VirtualServer.CommandArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                tbCommand.Text += $"{e.Command} with {e.Parameters}" + Environment.NewLine;
            });
        }


        private void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            _server.OnBattlEyeConnected(new BattlEyeConnectEventArgs(new BattlEyeLoginCredentials(), BattlEyeConnectionResult.Success));
        }

        private void Disconnect_OnClick(object sender, RoutedEventArgs e)
        {
            _server.OnBattlEyeDisconnected(new BattlEyeDisconnectEventArgs(new BattlEyeLoginCredentials(), BattlEyeDisconnectionType.Manual));
        }

        private int messageId = 0;
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            _server.OnBattlEyeMessageReceived(new BattlEyeMessageEventArgs(tbCmd.Text, ++messageId));
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            tbCommand.Text = string.Empty;
        }
    }
}
