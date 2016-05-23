using System;
using System.Windows;
using Arma3BE.Client.Modules.MainModule.Extensions;
using Arma3BE.Client.Modules.MainModule.Models;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BE.Client.Modules.MainModule.Boxes
{
    /// <summary>
    ///     Interaction logic for PlayerViewWindow.xaml
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


            DataContext = _model;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _model.Cleanup();
        }
    }
}