using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Arma3BEClient.Boxes;
using Arma3BEClient.Extensions;
using Arma3BEClient.Models;
using PlayerView = Arma3BEClient.Helpers.Views.PlayerView;

namespace Arma3BEClient.Grids
{
    /// <summary>
    ///     Interaction logic for OnlinePlayers.xaml
    /// </summary>
    public partial class OnlinePlayers : UserControl
    {
        public OnlinePlayers()
        {
            InitializeComponent();


            var menu = dg.Generate<PlayerView>();

            foreach (var menuItem in menu.Items.OfType<MenuItem>().ToList())
            {
                menu.Items.Remove(menuItem);
                dg.ContextMenu.Items.Add(menuItem);
            }

            foreach (var generateColumn in GridHelper.GenerateColumns<PlayerView>())
            {
                dg.Columns.Add(generateColumn);
            }
        }

        private ServerMonitorPlayerViewModel Model
        {
            get { return DataContext as ServerMonitorPlayerViewModel; }
        }

        private void KickClick(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as PlayerView;

            if (si != null)
            {
                var w = new KickPlayerWindow(Model._playerHelper, si);
                w.ShowDialog();
            }
        }

        private void BanClick(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as PlayerView;

            if (si != null)
            {
                var w = new BanPlayerWindow(Model._playerHelper, si.Guid, true, si.Name, si.Num.ToString());
                w.ShowDialog();
            }
        }

        private void PlayerInfo_Click(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as PlayerView;

            if (si != null)
            {
                var w = new PlayerViewWindow(new PlayerViewModel(si.Guid));
                w.ShowDialog();
            }
        }
    }
}