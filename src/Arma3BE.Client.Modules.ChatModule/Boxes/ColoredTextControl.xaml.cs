using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System;
using System.Collections.Generic;
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
        private Paragraph _paragraph;



        public static readonly DependencyProperty IsAutoScrollProperty =
            DependencyProperty.Register("IsAutoScroll", typeof(Boolean), typeof(ColoredTextControl),
                new FrameworkPropertyMetadata(false));


        public bool IsAutoScroll
        {
            get { return (bool)GetValue(IsAutoScrollProperty); }
            set { SetValue(IsAutoScrollProperty, value); }
        }

        public ColoredTextControl()
        {
            InitializeComponent();
            InitBox();
        }

        private void InitBox()
        {
            msgBox.Document.Blocks.Clear();
            _paragraph = new Paragraph();
            msgBox.Document.Blocks.Add(_paragraph);


            msgBox.IsDocumentEnabled = true;
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

            if (IsAutoScroll)
                scroll.ScrollToEnd();
        }

        public void AppendText(ChatMessage message, string servername)
        {
            AppendText(_paragraph, message, servername);
        }

        private void AppendText(Paragraph p, ChatMessage message, string servername)
        {
            var text = string.Format("[{0}] [ {1:yyyy-MM-dd HH:mm:ss} ]  {2}\n", servername, message.Date.UtcToLocalFromSettings(),
                message.Message);
            var color = ServerMonitorChatViewModel.GetMessageColor(message);
            var brush = new SolidColorBrush(color);
            var span = new Span { Foreground = brush };

            if (message.Type != ChatMessage.MessageType.RCon && message.IsImportantMessage)
            {
                span.FontWeight = FontWeights.Heavy;
            }

            span.Inlines.Add(text);
            p.Inlines.Add(span);
        }


        public void AppendPlayerText(KeyValuePair<string, string> player, bool isIn)
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
                if (string.IsNullOrEmpty(context) == false)
                {
                    var aggregator = ServiceLocator.Current.TryResolve<IEventAggregator>();
                    aggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(context));
                }
            };

            p.Inlines.Add(text);
            p.Inlines.Add(link);
            p.Inlines.Add(new Run(isIn ? " Connected" : " Disconnected") { FontSize = 10 });

            msgBox.Document.Blocks.Add(p);

            if (IsAutoScroll)
                ChatScrollViewer.ScrollToEnd();
        }

        public void ClearAll()
        {
            msgBox.Document.Blocks.Clear();
            InitBox();
            msgBox.AppendText(Environment.NewLine);
        }
    }
}
