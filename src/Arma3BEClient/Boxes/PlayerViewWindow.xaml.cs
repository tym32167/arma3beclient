using System.Windows;
using Arma3BEClient.Extensions;
using Arma3BEClient.Lib.ModelCompact;
using Arma3BEClient.Models;

namespace Arma3BEClient.Boxes
{
    /// <summary>
    /// Interaction logic for PlayerViewWindow.xaml
    /// </summary>
    public partial class PlayerViewWindow : Window
    {
        private readonly PlayerViewModel _model;

        public PlayerViewWindow(PlayerViewModel model)
        {
            _model = model;
            InitializeComponent();


            dgBans.ContextMenu = dgBans.Generate<Ban>();
            dgHist.ContextMenu = dgHist.Generate<PlayerHistory>();
            dgNotes.ContextMenu = dgNotes.Generate<Note>();


            this.DataContext = _model;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _model.Cleanup();
        }
    }
}
