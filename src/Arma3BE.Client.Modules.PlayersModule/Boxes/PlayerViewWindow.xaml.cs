using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Modules.PlayersModule.Models;
using Arma3BEClient.Libs.ModelCompact;
using System.Windows;
using Arma3BE.Client.Infrastructure.Windows;
using Arma3BEClient.Libs.Tools;

namespace Arma3BE.Client.Modules.PlayersModule.Boxes
{
    /// <summary>
    ///     Interaction logic for PlayerViewWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class PlayerViewWindow : WindowBase
    {
        public PlayerViewWindow(PlayerViewModel model, ISettingsStoreSource settingsStoreSource):base(settingsStoreSource)
        {
            InitializeComponent();

            dgBans.ContextMenu = dgBans.Generate<Ban>();
            dgHist.ContextMenu = dgHist.Generate<PlayerHistory>();
            dgNotes.ContextMenu = dgNotes.Generate<Note>();

            DataContext = model;
        }
    }
}