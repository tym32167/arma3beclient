using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Arma3BE.Server.Models;
using Arma3BEClient.Models;
using Arma3BEClient.ViewModel;

namespace Arma3BEClient.Chat
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
            Dispatcher.Invoke(() =>
            {
                if (!Model.EnableChat) return;
                var type = e.Message.Type;
                if (type != ChatMessage.MessageType.Unknown)
                    AppendText(_paragraph, ChatScrollViewer, e.Message);
                else
                    AppendText(msgConsole, ConsoleScrollViewer, e.Message);
            });
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
            _paragraph = new Paragraph();

            msgBox.Document.Blocks.Add(_paragraph);
        }

        public void AppendText(Paragraph p, ScrollViewer scroll, ChatMessage message)
        {
            var text = $"[ {message.Date:HH:mm:ss} ]  {message.Message}\n";
            var color = ServerMonitorModel.GetMessageColor(message);

            var brush = new SolidColorBrush(color);
            var span = new Span {Foreground = brush};
            span.Inlines.Add(text);
            _paragraph.Inlines.Add(span);

            if (Model.AutoScroll)
                scroll.ScrollToEnd();
        }

        public void AppendText(TextBox block, ScrollViewer scroll, ChatMessage message)
        {
            var text = $"[ {message.Date:HH:mm:ss} ]  {message.Message}\n";
            block.Text += text;

            if (Model.AutoScroll)
                scroll.ScrollToEnd();
        }

        private void ToolBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Model.ChatMessageEventHandler += _model_ChatMessageEventHandler;
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            msgBox.Document.Blocks.Clear();
            msgConsole.Text = string.Empty;

            InitBox();

            msgBox.AppendText(Environment.NewLine);
        }
    }
}