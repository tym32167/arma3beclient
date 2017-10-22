using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Server.Models;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Controls;

namespace Arma3BE.Client.Modules.ChatModule.Boxes
{
    /// <summary>
    /// Interaction logic for ColoredItemsControl.xaml
    /// </summary>
    public partial class ColoredItemsControl : UserControl, IColoredTextControl
    {
        public ColoredItemsControl()
        {
            InitializeComponent();
            DataContext = this;

            ShowPlayerCommand = new DelegateCommand<PlayerMessage>(p =>
            {
                var aggregator = ServiceLocator.Current.TryResolve<IEventAggregator>();
                aggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(p.PlayerId));
            });
        }

        public ICommand ShowPlayerCommand { get; set; }

        public void AppendPlayerText(KeyValuePair<string, string> player, bool isIn)
        {
            Messages.Add(new PlayerMessage { PlayerId = player.Key, PlayerName = player.Value, isIn = isIn, DateTime = DateTime.UtcNow.UtcToLocalFromSettings() });
            ScrollIfNeeded();
        }

        public void AppendText(ChatMessage message, string servername = null)
        {
            Messages.Add(new FullChatMessage { Message = message, Server = servername });
            ScrollIfNeeded();
        }

        public void ClearAll()
        {
            Messages.Clear();
        }

        void ScrollIfNeeded()
        {
            if (!IsAutoScroll) return;

            var sv = lv.FindVisualChildren<ScrollViewer>().FirstOrDefault();
            sv?.ScrollToEnd();
        }

        public ObservableCollection<object> Messages { get; set; } = new ObservableCollection<object>();

        public static readonly DependencyProperty IsAutoScrollProperty =
            DependencyProperty.Register("IsAutoScroll", typeof(Boolean), typeof(ColoredItemsControl),
                new FrameworkPropertyMetadata(false));


        public bool IsAutoScroll
        {
            get { return (bool)GetValue(IsAutoScrollProperty); }
            set { SetValue(IsAutoScrollProperty, value); }
        }
    }

    public class PlayerMessage
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool isIn { get; set; }
        public DateTime DateTime { get; set; }
    }


    public class FullChatMessage
    {
        public string Server { get; set; }
        public ChatMessage Message { get; set; }
    }



}
