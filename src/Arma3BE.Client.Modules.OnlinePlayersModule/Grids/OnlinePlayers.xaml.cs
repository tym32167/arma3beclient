using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.OnlinePlayersModule.Helpers.Views;
using Prism.Events;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Grids
{
    /// <summary>
    ///     Interaction logic for OnlinePlayers.xaml
    /// </summary>
    [ViewSortHint("0100")]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class OnlinePlayers : UserControl
    {
        private readonly IEventAggregator _eventAggregator;

        public OnlinePlayers(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();

            dg.LoadState<PlayerView>(GetType().FullName);

            var menu = dg.Generate<PlayerView>();

            foreach (var menuItem in menu.Items.OfType<MenuItem>().ToList())
            {
                menu.Items.Remove(menuItem);
                dg.ContextMenu?.Items.Add(menuItem);
            }
        }

        private void Dg_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var player = dg.SelectedItem as PlayerView;
            if (player != null)
            {
                _eventAggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(player.Guid));
            }
        }
    }
}