using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Arma3BEClient.Boxes;
using Arma3BEClient.Extensions;
using Arma3BEClient.Models;
using Arma3BEClient.ViewModel;

namespace Arma3BEClient.Grids
{
    /// <summary>
    /// Interaction logic for PlayersControl.xaml
    /// </summary>
    public partial class PlayersControl : UserControl
    {
        public PlayersControl()
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
      

        private PlayerListModelView Model { get { return DataContext as PlayerListModelView; } }

        private void BanClick(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as PlayerView;

            if (si != null)
            {
                var t = new Helpers.Views.PlayerView()
                {
                    Id = si.Id,
                    Name = si.Name,
                    Comment = si.Comment,
                    IP = si.LastIp,
                    Guid = si.Guid,
                };
                var w = new BanPlayerWindow(Model._playerHelper, si.Guid , false, si.Name, null);
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
