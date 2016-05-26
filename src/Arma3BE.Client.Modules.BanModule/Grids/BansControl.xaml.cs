using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Client.Modules.BanModule.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.DataGrid;

namespace Arma3BE.Client.Modules.BanModule.Grids
{
    /// <summary>
    ///     Interaction logic for BansControl.xaml
    /// </summary>
    public partial class BansControl : UserControl
    {
        public BansControl()
        {
            InitializeComponent();

            var menu = dg.Generate<BanView>();

            foreach (var menuItem in menu.Items.OfType<MenuItem>().ToList())
            {
                menu.Items.Remove(menuItem);
                dg.ContextMenu.Items.Add(menuItem);
            }

            foreach (var generateColumn in GridHelper.GenerateColumns<BanView>())
            {
                dg.Columns.Add(generateColumn);
            }


            var menu2 = dg2.Generate<BanView>();

            foreach (var menuItem in menu2.Items.OfType<MenuItem>().ToList())
            {
                menu2.Items.Remove(menuItem);
                dg2.ContextMenu.Items.Add(menuItem);
            }

            foreach (var generateColumn in GridHelper.GenerateColumns<BanView>())
            {
                dg2.Columns.Add(generateColumn);
            }

            //_playerViewService = MainModuleInit.Current.Resolve<IPlayerViewService>();
        }

        //private IPlayerViewService _playerViewService;

        private ServerMonitorBansViewModel Model
        {
            get { return DataContext as ServerMonitorBansViewModel; }
        }

        private void RemoveBan_Click(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as BanView;

            if (si != null)
            {
                Model.RemoveBan(si);
            }
        }

        private void Av_Grid_Selection_Changed(object sender, DataGridSelectionChangedEventArgs e)
        {
            var dataGridControl = e.OriginalSource as DataGridControl;
            if (dataGridControl != null)
            {
                var res =
                    (from object selectedItem in dataGridControl.SelectedItems select selectedItem as BanView).ToList();

                Model.SelectedAvailibleBans = res;
            }
        }

        private void PlayerInfo_Click(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as BanView;

            if (si != null)
            {
                //_playerViewService.ShowDialog(si.GuidIp);
            }
        }

        private void PlayerInfo2_Click(object sender, RoutedEventArgs e)
        {
            var si = dg2.SelectedItem as BanView;

            if (si != null)
            {
                // _playerViewService.ShowDialog(si.GuidIp);
            }
        }
    }
}