using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Arma3BEClient.Models;
using Arma3BEClient.Updater.Models;
using Arma3BEClient.ViewModel;

namespace Arma3BEClient.Chat
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl{


        private ServerMonitorChatViewModel Model { get { return DataContext as ServerMonitorChatViewModel; } }

        public ChatControl()
        {
            InitializeComponent();
        }

        void _model_ChatMessageEventHandler(object sender, Updater.Models.ChatMessage e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!Model.EnableChat) return;
                var type = e.Type;
                if (type != ChatMessage.MessageType.Unknown)
                    AppendText(msgBox, ChatScrollViewer, e);
                else
                    AppendText(msgConsole, ConsoleScrollViewer, e);
            });
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            Model.SendMessage();
        }


        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = tbNewMessage.Text;
                Model.SendMessage(text);
            }
        }

        public void AppendText(RichTextBox box, ScrollViewer scroll, ChatMessage message)
        {
            var text = string.Format("[ {0:HH:mm:ss} ]  {1}\n", message.Date, message.Message);
            var color = ServerMonitorModel.GetMessageColor(message);
            var tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

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
            msgConsole.Document.Blocks.Clear();

            msgBox.AppendText(Environment.NewLine);
            msgConsole.AppendText(Environment.NewLine);
        }
    }
}
