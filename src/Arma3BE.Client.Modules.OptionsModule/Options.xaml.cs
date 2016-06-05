using System;
using System.Threading.Tasks;
using System.Windows;
using Arma3BE.Client.Modules.OptionsModule.ViewModel;
using Xceed.Wpf.Toolkit;

namespace Arma3BE.Client.Modules.OptionsModule
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            var t = Task.Factory.StartNew(() =>
            {
                _optionsModel.Save();
            });

            t.ContinueWith((p) =>
            {
                Dispatcher.Invoke(() =>
                {
                    BusyIndicator.IsBusy = false;
                    Close();
                });
            });
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}