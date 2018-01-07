using Arma3BE.Client.Infrastructure.Windows;
using Arma3BE.Client.Modules.OptionsModule.ViewModel;
using Arma3BEClient.Libs.Tools;
using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Arma3BE.Client.Modules.OptionsModule
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class Options : WindowBase
    {
        private readonly OptionsModel _optionsModel;

        public Options(OptionsModel optionsModel, ISettingsStoreSource settingsStoreSource) : base(settingsStoreSource)
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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            await _optionsModel.Save();
            BusyIndicator.IsBusy = false;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}