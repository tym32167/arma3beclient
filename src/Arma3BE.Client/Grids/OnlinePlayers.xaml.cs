using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Arma3BEClient.Boxes;
using Arma3BEClient.Extensions;
using Arma3BEClient.Models;

namespace Arma3BEClient.Grids
{
    /// <summary>
    /// Interaction logic for OnlinePlayers.xaml
    /// </summary>
    public partial class OnlinePlayers : UserControl
    {
        public OnlinePlayers()
        {
            InitializeComponent();


            var menu = dg.Generate<Helpers.Views.PlayerView>();

            foreach (var menuItem in menu.Items.OfType<MenuItem>().ToList())
            {
                menu.Items.Remove(menuItem);
                dg.ContextMenu.Items.Add(menuItem);
            }

            foreach (var generateColumn in GridHelper.GenerateColumns<Helpers.Views.PlayerView>())
            {
                dg.Columns.Add(generateColumn);
            }
        }

        private void KickClick(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as Helpers.Views.PlayerView;

            if (si != null)
            {
                var w = new KickPlayerWindow(Model._playerHelper, si);
                w.ShowDialog();
            }
        }

        private ServerMonitorPlayerViewModel Model { get { return DataContext as ServerMonitorPlayerViewModel; } }

        private void BanClick(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as Helpers.Views.PlayerView;

            if (si != null)
            {
                var w = new BanPlayerWindow(Model._playerHelper, si.Guid, true, si.Name, si.Num.ToString());
                w.ShowDialog();
            }
        }

        private void PlayerInfo_Click(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as Helpers.Views.PlayerView;

            if (si != null)
            {
                var w = new PlayerViewWindow(new PlayerViewModel(si.Guid));
                w.ShowDialog();
            }
        }
    }
}
