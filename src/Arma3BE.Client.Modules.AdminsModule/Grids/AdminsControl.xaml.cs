using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Server.Models;
using Prism.Regions;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.AdminsModule.Grids
{
    /// <summary>
    ///     Interaction logic for AdminsControl.xaml
    /// </summary>
    [ViewSortHint("0300")]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class AdminsControl : UserControl
    {
        public AdminsControl()
        {
            InitializeComponent();
            dg.ContextMenu = dg.Generate<Admin>();
            dg.LoadState<Admin>(GetType().FullName);
        }
    }
}