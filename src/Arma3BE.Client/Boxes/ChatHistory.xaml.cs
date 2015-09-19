using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Arma3BE.Server.Models;
using Arma3BEClient.Models;
using Arma3BEClient.ViewModel;

namespace Arma3BEClient.Boxes
{
    /// <summary>
    ///     Interaction logic for ChatHistory.xaml
    /// </summary>
    public partial class ChatHistory : Window
    {
        private readonly ChatHistoryViewModel _model;

        public ChatHistory(ChatHistoryViewModel model)
        {
            _model = model;
            InitializeComponent();

            _model.PropertyChanged += _model_PropertyChanged;

            DataContext = _model;
        }

        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Log")
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            msgBox.Document.Blocks.Clear();
            var p = new Paragraph();

            foreach (var chatLog in _model.Log)
            {
                var mes = new ChatMessage {Date = chatLog.Date, Message = chatLog.Text};
                AppendText(p, ChatScrollViewer, mes, chatLog.ServerName);
            }


            msgBox.Document.Blocks.Add(p);
        }

        public void AppendText(Paragraph p, ScrollViewer scroll, ChatMessage message, string servername)
        {
            var text = string.Format("[{0}] [ {1:yyyy-MM-dd HH:mm:ss} ]  {2}\n", servername, message.Date,
                message.Message);
            var color = ServerMonitorModel.GetMessageColor(message);
            var brush = new SolidColorBrush(color);
            var span = new Span {Foreground = brush};
            span.Inlines.Add(text);
            p.Inlines.Add(span);
        }
    }
}