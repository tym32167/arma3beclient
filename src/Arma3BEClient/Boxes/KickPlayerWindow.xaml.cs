using System.Windows;
using Arma3BEClient.Helpers;
using Arma3BEClient.Models;
using GalaSoft.MvvmLight;
using PlayerView = Arma3BEClient.Helpers.Views.PlayerView;

namespace Arma3BEClient.Boxes
{
    /// <summary>
    /// Interaction logic for KickPlayerWindow.xaml
    /// </summary>
    public partial class KickPlayerWindow : Window
    {
        private readonly PlayerHelper _playerHelper;
        private readonly Helpers.Views.PlayerView _playerView;

        private KickPlayerViewModel Model;

        public KickPlayerWindow(PlayerHelper playerHelper, Helpers.Views.PlayerView playerView)
        {
            _playerHelper = playerHelper;
            _playerView = playerView;
            InitializeComponent();

            Model = new KickPlayerViewModel(playerView);

            this.DataContext = Model;
        }

        private async void KickClick(object sender, RoutedEventArgs e)
        {
            await _playerHelper.KickAsync(_playerView, tbReason.Text);
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }


    public class KickPlayerViewModel : ViewModelBase
    {
        private readonly PlayerView _playerView;

        public KickPlayerViewModel(Helpers.Views.PlayerView playerView)
        {
            _playerView = playerView;
        }


        public PlayerView Player { get { return _playerView; } }

        private string _reason;

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                RaisePropertyChanged("Reason");
            }
        }
    }
}
