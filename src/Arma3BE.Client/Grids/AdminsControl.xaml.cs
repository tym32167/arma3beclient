using System.Windows.Controls;
using Arma3BEClient.Extensions;
using Arma3BEClient.Updater.Models;

namespace Arma3BEClient.Grids
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