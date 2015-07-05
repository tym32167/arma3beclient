using System;
using System.Linq;
using System.Windows;
using Arma3BEClient.ViewModel;
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
            var dbm = model.GetDbModel();

            if (dbm.Id != Guid.Empty)
            {
                _optionsModel.Local.Remove(dbm);
            }

            _optionsModel.Servers.Remove(e.Item as ServerInfoModel);
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
                if (dbm.ChatLog.Any()) e.Cancel = true;
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