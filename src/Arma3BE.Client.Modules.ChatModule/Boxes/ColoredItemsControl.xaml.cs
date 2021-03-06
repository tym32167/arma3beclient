﻿using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Repositories;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            InitBox();

            this.IsHitTestVisibleChanged += ColoredItemsControl_IsVisibleChanged;
            this.IsVisibleChanged += ColoredItemsControl_IsVisibleChanged;
        }

        private void ColoredItemsControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            lv.SelectedItem = null;
            ScrollIfNeeded();
        }

        private async Task InitBox()
        {
            using (var repo = new ReasonRepository())
            {
                importantWords = await repo.GetImportantWordsAsync();
            }
        }

        private string[] importantWords;

        bool IsImportantMessage(ChatMessage message)
        {
            if (importantWords?.Any() == false) return false;

            return ChatMessage.IsImportantMessageProc(message, importantWords);
        }

        public ICommand ShowPlayerCommand { get; set; }

        public void AppendPlayerText(KeyValuePair<string, string> player, bool isIn)
        {
            Messages.Add(new PlayerMessage { PlayerId = player.Key, PlayerName = player.Value, isIn = isIn, DateTime = DateTime.UtcNow.UtcToLocalFromSettings() });
            ScrollIfNeeded();
        }

        public void AppendText(ChatMessage message, string servername = null)
        {
            var msg = new FullChatMessage
            {
                Message = message,
                Server = servername,
                DateTime = message.Date.UtcToLocalFromSettings(),
                IsImportant = message.Type != ChatMessage.MessageType.RCon && IsImportantMessage(message)
            };

            if (msg.IsImportant)
            {
                if (Application.Current?.MainWindow?.WindowState == WindowState.Minimized)
                {
                    var wnd = Application.Current.MainWindow;

                    EventHandler handler = null;
                    handler = (s, e) =>
                    {
                        wnd.StateChanged -= handler;
                        wnd.StopFlashingWindow();
                    };

                    wnd.StateChanged += handler;
                    wnd.FlashWindow();
                }
            }


            Messages.Add(msg);
            ScrollIfNeeded();
        }

        public void ClearAll()
        {
            Messages.Clear();
        }

        void ScrollIfNeeded()
        {
            if (!IsAutoScroll) return;

            lv.SelectedIndex = lv.Items.Count - 1;
            lv.ScrollIntoView(lv.SelectedItem);
            lv.SelectedItem = null;
        }

        void OnListCopy(object sender, ExecutedRoutedEventArgs e)
        {
            var lv = (ListView)sender;
            var text = string.Join(Environment.NewLine, lv.SelectedItems.Cast<object>()
                .OrderBy(x => Messages.IndexOf(x))
                .Select(x => x.ToString()).ToArray());
            Clipboard.SetDataObject(text);
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

        public void AppendText(LogMessage message)
        {
            var text = $"[ {message.Date.UtcToLocalFromSettings():HH:mm:ss} ]  {message.Message}";
            Messages.Add(text);
            ScrollIfNeeded();
        }
    }

    public class PlayerMessage
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool isIn { get; set; }
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            var text = $"[ {DateTime.UtcNow.UtcToLocalFromSettings():HH:mm:ss} ] {PlayerName} {(isIn ? "connected" : "disconnected")}";
            return text;
        }
    }


    public class FullChatMessage
    {
        public string Server { get; set; }
        public DateTime DateTime { get; set; }
        public ChatMessage Message { get; set; }
        public bool IsImportant { get; set; }

        public override string ToString()
        {
            var ret = string.IsNullOrEmpty(Server)
                ? $"[ {Message.Date.UtcToLocalFromSettings():HH:mm:ss} ]  {Message.Message}"
                : $"[{Server}] [ {Message.Date.UtcToLocalFromSettings():yyyy-MM-dd HH:mm:ss} ]  {Message.Message}";
            return ret;
        }
    }
}
