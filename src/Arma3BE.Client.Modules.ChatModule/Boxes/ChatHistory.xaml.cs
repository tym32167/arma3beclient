using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BE.Server.Models;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Arma3BE.Client.Infrastructure.Windows;
using Arma3BEClient.Libs.Core.Settings;
using Arma3BEClient.Libs.Tools;

namespace Arma3BE.Client.Modules.ChatModule.Boxes
{
    /// <summary>
    ///     Interaction logic for ChatHistory.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class ChatHistory : WindowBase
    {
        private readonly ChatHistoryViewModel _model;

        public ChatHistory(ChatHistoryViewModel model, ISettingsStoreSource settingsStoreSource):base(settingsStoreSource)
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
            textControl.ClearAll();

            if (_model.Log.Count() > 10000)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                MessageBox.Show(GetWindow(this), "Too many results.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var chatLog in _model.Log)
            {
                var mes = new ChatMessage { Date = chatLog.Date, Message = chatLog.Text };
                textControl.AppendText(mes, chatLog.ServerName);
            }
        }
    }
}