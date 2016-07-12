using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.OnlinePlayersModule.Helpers.Views;
using System.Linq;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Grids
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
    }
}