using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.PlayersModule.Models;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.PlayersModule.Grids
{
    /// <summary>
    ///     Interaction logic for PlayersControl.xaml
    /// </summary>
    [ViewSortHint("0400")]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class PlayersControl : UserControl
    {
        public PlayersControl()
        {
            InitializeComponent();

            var menu = dg.Generate<PlayerView>();

            foreach (var menuItem in menu.Items.OfType<MenuItem>().ToList())
            {
                menu.Items.Remove(menuItem);
                dg.ContextMenu?.Items.Add(menuItem);
            }

            dg.LoadState<PlayerView>(GetType().FullName);
        }
    }
}