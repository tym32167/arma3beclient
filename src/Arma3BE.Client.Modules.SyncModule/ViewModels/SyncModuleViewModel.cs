using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.SyncModule.SyncCore;
using Arma3BEClient.Libs.Tools;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.SyncModule.ViewModels
{
    public class SyncModuleViewModel : ViewModelBase, ITitledItem
    {
        private readonly SyncWorker _syncWorker;
        private readonly ISettingsStoreSource _settingsStoreSource;
        public static string StaticTitle = "Sync Service";
        public string Title => StaticTitle;

        public SyncModuleViewModel(SyncWorker syncWorker, ISettingsStoreSource settingsStoreSource)
        {
            _syncWorker = syncWorker;
            _settingsStoreSource = settingsStoreSource;
            SyncCommand = new ActionCommand(Sync, () => !_isSyncing);
            CancelCommand = new ActionCommand(() =>
            {
                _tokenSource?.Cancel();
                _tokenSource = null;
            }, () => _isSyncing);
            SaveCommand = new ActionCommand(Save);
            Load();
        }

        private string _userName;
        private string _userPassword;
        private string _endpoint;
        private int _progress;

        private void Load()
        {
            var store = _settingsStoreSource.GetCustomSettingsStore();
            UserName = store.Load(SettingsKeys.UserNamePrefix);
            Endpoint = store.Load(SettingsKeys.EndpointPrefix);
        }

        private void Save()
        {
            var store = _settingsStoreSource.GetCustomSettingsStore();
            store.Save(SettingsKeys.EndpointPrefix, Endpoint);
            store.Save(SettingsKeys.UserNamePrefix, UserName);
        }


        private bool _isSyncing = false;
        private CancellationTokenSource _tokenSource;
        private async void Sync()
        {
            Save();
            try
            {
                _isSyncing = true;
                CommandManager.InvalidateRequerySuggested();
                SyncCommand.CanExecuteInvalidate();
                CancelCommand.CanExecuteInvalidate();

                var progress = new Progress<int>();
                progress.ProgressChanged += (s, e) => Progress = e;
                Progress = 0;

                _tokenSource = new CancellationTokenSource();

                var result = await _syncWorker.Sync(new SyncCredentials(Endpoint, UserName, UserPassword),
                    _tokenSource.Token,
                    progress);

                MessageBox.Show($"Added {result.Added}, Updated {result.Updated}", "Sync Result", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            finally
            {
                _isSyncing = false;
                CommandManager.InvalidateRequerySuggested();
                SyncCommand.CanExecuteInvalidate();
                CancelCommand.CanExecuteInvalidate();
                _tokenSource?.Cancel();
                _tokenSource = null;
            }
        }


        public ActionCommand SyncCommand { get; set; }
        public ActionCommand CancelCommand { get; set; }

        public ICommand SaveCommand { get; set; }


        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        public string Endpoint
        {
            get => _endpoint;
            set
            {
                if (value == _endpoint) return;
                _endpoint = value;
                RaisePropertyChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (value == _userName) return;
                _userName = value;
                RaisePropertyChanged();
            }
        }

        public string UserPassword
        {
            get => _userPassword;
            set
            {
                if (value == _userPassword) return;
                _userPassword = value;
                RaisePropertyChanged();
            }
        }
    }
}