using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.ChatModule.Boxes
{
    /// <summary>
    ///     Interaction logic for RTBControl.xaml
    /// </summary>
    public partial class RTBControl : UserControl, IColoredTextControl
    {
        public static readonly DependencyProperty IsAutoScrollProperty =
            DependencyProperty.Register("IsAutoScroll", typeof(bool), typeof(RTBControl),
                new FrameworkPropertyMetadata(false));

        public RTBControl()
        {
            InitializeComponent();

            rtb.Font = new Font(rtb.Font.FontFamily, 12);
        }


        public bool IsAutoScroll
        {
            get { return (bool)GetValue(IsAutoScrollProperty); }
            set { SetValue(IsAutoScrollProperty, value); }
        }

        public void AppendPlayerText(KeyValuePair<string, string> player, bool isIn)
        {
        }



        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        const int WM_USER = 0x400;
        const int EM_HIDESELECTION = WM_USER + 63;

        public void AppendText(ChatMessage message, string servername = null)
        {
            string text;

            if (string.IsNullOrEmpty(servername))
            {
                text = $"[ {message.Date.UtcToLocalFromSettings():HH:mm:ss} ]  {message.Message}\n";
            }
            else
            {
                text =
                    $"[{servername}] [ {message.Date.UtcToLocalFromSettings():yyyy-MM-dd HH:mm:ss} ]  {message.Message}\n";
            }

            var color = ServerMonitorChatViewModel.GetMessageColor(message);
            var wformsColor = Color.FromArgb(color.A, color.R, color.G, color.B);



            var selection = rtb.SelectionStart;
            var length = rtb.SelectionLength;


            if (!IsAutoScroll)
            {
                SendMessage(rtb.Handle, EM_HIDESELECTION, 1, 0);
            }


            rtb.AppendText(text);

            rtb.Select(rtb.TextLength - text.Length, text.Length);
            rtb.SelectionColor = wformsColor;


            if (message.Type != ChatMessage.MessageType.RCon && message.IsImportantMessage)
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
            {
                SendMessage(rtb.Handle, EM_HIDESELECTION, 0, 0);
            }
        }

        public void ClearAll()
        {
            rtb.Clear();
        }
    }
}