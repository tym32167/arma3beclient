using System;
using System.Linq;
using System.Windows;
using Arma3BEClient.Libs.Repositories;
using Arma3BEClient.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;

namespace Arma3BEClient
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private readonly OptionsModel _optionsModel;

        public Options(OptionsModel optionsModel)
        {
            InitializeComponent();

            _optionsModel = optionsModel;

            DataContext = _optionsModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _optionsModel.Cleanup();
        }

        private void ServerDeleted(object sender, ItemEventArgs e)
        {
            var model = e.Item as ServerInfoModel;
            _optionsModel.Servers.Remove(model);
        }

        private void ServerAdded(object sender, ItemEventArgs e)
        {
            _optionsModel.Servers.Add(e.Item as ServerInfoModel);
        }

        private void ServerDeleting(object sender, ItemDeletingEventArgs e)
        {
            var model = e.Item as ServerInfoModel;
            var dbm = model.GetDbModel();
            if (dbm.Id != Guid.Empty)
            {
                using (var chat = new ChatRepository())
                {
                    if (chat.HaveChatLogs(dbm.Id)) e.Cancel = true;
                    
                }
                
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _optionsModel.Save();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}