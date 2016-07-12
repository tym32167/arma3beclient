using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Server.Models;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.AdminsModule.Grids
{
    /// <summary>
    ///     Interaction logic for AdminsControl.xaml
    /// </summary>
    public partial class AdminsControl : UserControl
    {
        public AdminsControl()
        {
            InitializeComponent();


            dg.ContextMenu = dg.Generate<Admin>();

            foreach (var generateColumn in GridHelper.GenerateColumns<Admin>())
            {
                dg.Columns.Add(generateColumn);
            }
        }
    }
}