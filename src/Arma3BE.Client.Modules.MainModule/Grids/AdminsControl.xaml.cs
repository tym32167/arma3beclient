using System.Windows.Controls;
using Arma3BE.Client.Modules.MainModule.Extensions;
using Arma3BE.Server.Models;

namespace Arma3BE.Client.Modules.MainModule.Grids
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