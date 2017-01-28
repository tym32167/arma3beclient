using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.ChatModule.Chat
{
    /// <summary>
    ///     Interaction logic for ChatControl.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class ChatControl : UserControl
    {
        public ChatControl()
        {
            InitializeComponent();
        }

        private ServerMonitorChatViewModel Model => DataContext as ServerMonitorChatViewModel;

        private void _model_ChatMessageEventHandler(object sender, ServerMonitorChatViewModelEventArgs e)
        {
            if (!Model.EnableChat) return;
            var type = e.Message.Type;
            if (type != ChatMessage.MessageType.Unknown)
                textControl.AppendText(e.Message);
            else
                AppendText(msgConsole, ConsoleScrollViewer, e.Message);
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            Model.SendMessage();
        }

        private new void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = tbNewMessage.Text;
                Model.SendMessage(text);
            }
        }

        public void AppendText(TextBox block, ScrollViewer scroll, MessageBase message)
        {
            var text = $"[ {message.Date.UtcToLocalFromSettings():HH:mm:ss} ]  {message.Message}\n";
            block.Text += text;

            if (Model.AutoScroll)
                scroll.ScrollToEnd();
        }

        private void ToolBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var model = e.NewValue as ServerMonitorChatViewModel;
            if (model != null)
            {
                model.ChatMessageEventHandler += _model_ChatMessageEventHandler;
                model.LogMessageEventHandler += Model_LogMessageEventHandler;

                model.PlayersInHandler += ModelOnPlayersInHandler;
                model.PlayersOutHandler += ModelOnPlayersOutHandler;
            }
        }

        private void ModelOnPlayersOutHandler(object sender, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!Model.EnableChat) return;
            foreach (var pair in keyValuePairs)
            {
                textPlayerControl.AppendPlayerText(pair, false);
            }

        }

        private void ModelOnPlayersInHandler(object sender, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!Model.EnableChat) return;
            foreach (var pair in keyValuePairs)
            {
                textPlayerControl.AppendPlayerText(pair, true);
            }
        }

        private void Model_LogMessageEventHandler(object sender, ServerMonitorLogViewModelEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!Model.EnableChat) return;
                AppendText(msgConsole, ConsoleScrollViewer, e.Message);
            });
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            textPlayerControl.ClearAll();
            msgConsole.Text = string.Empty;
            textControl.ClearAll();
        }
    }
}