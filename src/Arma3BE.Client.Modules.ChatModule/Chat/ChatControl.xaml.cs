using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Arma3BE.Client.Modules.ChatModule.Chat
{
    /// <summary>
    ///     Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        private Paragraph _paragraph;

        public ChatControl()
        {
            InitializeComponent();
            InitBox();
        }

        private ServerMonitorChatViewModel Model => DataContext as ServerMonitorChatViewModel;

        private void _model_ChatMessageEventHandler(object sender, ServerMonitorChatViewModelEventArgs e)
        {
            if (!Model.EnableChat) return;
            var type = e.Message.Type;
            if (type != ChatMessage.MessageType.Unknown)
                AppendText(_paragraph, ChatScrollViewer, e.Message);
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

        private void InitBox()
        {
            msgBox.Document.Blocks.Clear();
            histBox.Document.Blocks.Clear();

            _paragraph = new Paragraph();

            msgBox.Document.Blocks.Add(_paragraph);


            histBox.IsDocumentEnabled = true;
        }

        public void AppendText(Paragraph p, ScrollViewer scroll, ChatMessage message)
        {
            var text = $"[ {message.Date.UtcToLocalFromSettings():HH:mm:ss} ]  {message.Message}\n";
            var color = ServerMonitorChatViewModel.GetMessageColor(message);

            var brush = new SolidColorBrush(color);
            var span = new Span { Foreground = brush };
            span.Inlines.Add(text);

            if (message.Type != ChatMessage.MessageType.RCon && message.IsImportantMessage)
                span.FontWeight = FontWeights.Heavy;

            p.Inlines.Add(span);

            if (Model.AutoScroll)
                scroll.ScrollToEnd();
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
                AppendPlayerText(HistoryScrollViewer, pair, false);
            }

        }

        private void ModelOnPlayersInHandler(object sender, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!Model.EnableChat) return;
            foreach (var pair in keyValuePairs)
            {
                AppendPlayerText(HistoryScrollViewer, pair, true);
            }
        }



        public void AppendPlayerText(ScrollViewer scroll, KeyValuePair<string, string> player, bool isIn)
        {
            var text = $"[ {DateTime.UtcNow.UtcToLocalFromSettings():HH:mm:ss} ] ";

            var p = new Paragraph
            {
                Margin = new Thickness(0),
                Foreground = new SolidColorBrush(isIn ? Colors.Green : Colors.Red),
                FontSize = 13
            };
            // remove indent between paragraphs

            Hyperlink link = new Hyperlink { IsEnabled = true };
            link.DataContext = player.Key;
            link.Inlines.Add(player.Value);
            link.NavigateUri = new Uri("http://local/");
            link.RequestNavigate += (sender, args) =>
            {
                var context = (sender as FrameworkContentElement)?.DataContext as string;
                Model.ShowPlayer(context);
            };

            p.Inlines.Add(text);
            p.Inlines.Add(link);
            p.Inlines.Add(new Run(isIn ? " Connected" : " Disconnected") { FontSize = 10 });

            histBox.Document.Blocks.Add(p);


            if (Model.AutoScroll)
                scroll.ScrollToEnd();
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
            msgBox.Document.Blocks.Clear();
            histBox.Document.Blocks.Clear();


            msgConsole.Text = string.Empty;

            InitBox();

            msgBox.AppendText(Environment.NewLine);
        }
    }
}