using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Arma3BE.Client.Modules.ChatModule.Boxes
{
    /// <summary>
    ///     Interaction logic for RTBControl.xaml
    /// </summary>
    public partial class RTBControl : UserControl, IColoredTextControl
    {
        private const int WM_USER = 0x400;
        private const int EM_HIDESELECTION = WM_USER + 63;

        public static readonly DependencyProperty IsAutoScrollProperty =
            DependencyProperty.Register("IsAutoScroll", typeof(bool), typeof(RTBControl),
                new FrameworkPropertyMetadata(false));

        public RTBControl()
        {
            InitializeComponent();

            rtb.Font = new Font(rtb.Font.FontFamily, 10);

            rtb.LinkClicked += Rtb_LinkClicked;
        }

        private void Rtb_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
        {
            var id = e.LinkText.Replace("http://", string.Empty);
            var aggregator = ServiceLocator.Current.TryResolve<IEventAggregator>();
            aggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(id));
        }

        public bool IsAutoScroll
        {
            get { return (bool)GetValue(IsAutoScrollProperty); }
            set { SetValue(IsAutoScrollProperty, value); }
        }

        public void AppendPlayerText(KeyValuePair<string, string> player, bool isIn)
        {
            var text = $"[ {DateTime.UtcNow.UtcToLocalFromSettings():HH:mm:ss} ] {player.Value} (http://{player.Key})\n";
            var color = isIn ? Colors.Green : Colors.Red;
            Add(text, color);
        }


        public void AppendText(ChatMessage message, string servername = null)
        {
            string text;

            if (string.IsNullOrEmpty(servername))
                text = $"[ {message.Date.UtcToLocalFromSettings():HH:mm:ss} ]  {message.Message}\n";
            else
                text =
                    $"[{servername}] [ {message.Date.UtcToLocalFromSettings():yyyy-MM-dd HH:mm:ss} ]  {message.Message}\n";

            var color = ServerMonitorChatViewModel.GetMessageColor(message);
            Add(text, color, (message.Type != ChatMessage.MessageType.RCon) && message.IsImportantMessage);
        }


        public void ClearAll()
        {
            rtb.Clear();
        }


        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private void Add(string text, Color color, bool bold = false)
        {
            var wformsColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

            var selection = rtb.SelectionStart;
            var length = rtb.SelectionLength;


            if (!IsAutoScroll)
                SendMessage(rtb.Handle, EM_HIDESELECTION, 1, 0);


            rtb.AppendText(text);

            rtb.Select(rtb.TextLength - text.Length, text.Length);
            rtb.SelectionColor = wformsColor;


            if (bold)
                rtb.SelectionFont = new Font(rtb.SelectionFont, System.Drawing.FontStyle.Bold);

            if (length != 0)
            {
                rtb.SelectionStart = selection;
                rtb.SelectionLength = length;
            }
            else
            {
                rtb.Select(rtb.TextLength, 0);
            }


            if (!IsAutoScroll)
                SendMessage(rtb.Handle, EM_HIDESELECTION, 0, 0);
        }
    }
}