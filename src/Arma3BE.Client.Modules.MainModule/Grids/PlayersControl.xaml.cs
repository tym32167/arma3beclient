using Arma3BE.Client.Modules.MainModule.Boxes;
using Arma3BE.Client.Modules.MainModule.Dialogs;
using Arma3BE.Client.Modules.MainModule.Extensions;
using Arma3BE.Client.Modules.MainModule.Models;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.MainModule.Grids
{
    /// <summary>
    ///     Interaction logic for PlayersControl.xaml
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


            //var menu = dg.DgGenerate<PlayerView>();

            //foreach (var menuItem in menu.Items.OfType<MenuItem>().ToList())
            //{
            //    menu.Items.Remove(menuItem);
            //    dg.ContextMenu.Items.AddOrUpdate(menuItem);
            //}


            //foreach (var generateColumn in GridHelper.DgGenerateColumns<PlayerView>())
            //{
            //    dg.Columns.AddOrUpdate(generateColumn);
            //}
            _playerViewService = MainModuleInit.Current.Resolve<IPlayerViewService>();
        }

        private IPlayerViewService _playerViewService;

        private PlayerListModelView Model
        {
            get { return DataContext as PlayerListModelView; }
        }

        private void BanClick(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as PlayerView;

            if (si != null)
            {
                var w = new BanPlayerWindow(Model.PlayerHelper, si.Guid, false, si.Name, null);
                w.ShowDialog();
            }
        }

        private void PlayerInfo_Click(object sender, RoutedEventArgs e)
        {
            var si = dg.SelectedItem as PlayerView;

            if (si != null)
            {
                _playerViewService.ShowDialog(si.Guid);
            }
        }
    }
}