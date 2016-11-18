using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Arma3BE.Client.Modules.ChatModule.Boxes
{
    /// <summary>
    /// Interaction logic for ColoredTextControl.xaml
    /// </summary>
    public partial class ColoredTextControl : UserControl
    {
        private ServerMonitorChatViewModel Model => DataContext as ServerMonitorChatViewModel;
        private Paragraph _paragraph;

        public ColoredTextControl()
        {
            InitializeComponent();
            InitBox();
        }


        private void InitBox()
        {
            msgBox.Document.Blocks.Clear();
            msgBox.Document.Blocks.Add(_paragraph);
        }


        public void AppendText(ChatMessage message)
        {
            AppendText(_paragraph, ChatScrollViewer, message);
        }

        private void AppendText(Paragraph p, ScrollViewer scroll, ChatMessage message)
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

        public void ClearAll()
        {
            msgBox.Document.Blocks.Clear();
            InitBox();
            msgBox.AppendText(Environment.NewLine);
        }
    }
}
