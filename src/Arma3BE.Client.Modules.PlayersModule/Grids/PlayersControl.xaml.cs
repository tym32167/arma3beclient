using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.PlayersModule.Models;
using Prism.Events;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.PlayersModule.Grids
{
    /// <summary>
    ///     Interaction logic for PlayersControl.xaml
    /// </summary>
    [ViewSortHint("0400")]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class PlayersControl : UserControl
    {
        private readonly IEventAggregator _eventAggregator;

        public PlayersControl(IEventAggregator eventAggregator)
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