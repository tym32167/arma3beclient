using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.SteamModule.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using Prism.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arma3BE.Client.Modules.SteamModule.Models
{
    public class SteamServiceViewModel : DisposableViewModelBase, ITitledItem
    {
        private readonly ISettingsStoreSource _settingsStoreSource;
        public static string StaticTitle = "Steam Service";
        private string _folder;
        public string Title => StaticTitle;

        public SteamServiceViewModel(ISettingsStoreSource settingsStoreSource)
        {
            _settingsStoreSource = settingsStoreSource;
            SelectFolderCommand = new DelegateCommand(SelectFolder, () => !InProgress);
            GenerateDataCommand = new DelegateCommand(GenerateData, () => !string.IsNullOrEmpty(Folder) && !InProgress);

            CancelCommand = new DelegateCommand(Cancel, () => InProgress);

            var settings = _settingsStoreSource.GetSettingsStore();
            Folder = settings.SteamFolder;
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                if (SetProperty(ref _inProgress, value))
                {
                    CancelCommand.RaiseCanExecuteChanged();
                    GenerateDataCommand.RaiseCanExecuteChanged();
                    SelectFolderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Folder
        {
            get { return _folder; }
            set
            {
                if (SetProperty(ref _folder, value))
                {
                    GenerateDataCommand.RaiseCanExecuteChanged();

                    var settings = _settingsStoreSource.GetSettingsStore();
                    settings.SteamFolder = Folder;
                    settings.Save();
                }
            }
        }

        public int Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        public DelegateCommand SelectFolderCommand { get; set; }
        public DelegateCommand GenerateDataCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private void SelectFolder()
        {
            var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                Folder = fbd.SelectedPath;
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private bool _inProgress;
        private int _progress;

        private void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async void GenerateData()
        {
            InProgress = true;
            Progress = 0;

            var progress = new Progress<int>();
            progress.ProgressChanged += Progress_ProgressChanged;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                var log = new Log();
                var md5 = new OptimizedHashProviderFactory();
                var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);

                try
                {
                    generator.GenerateFile(Folder, progress, _cancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    MessageBox.Show(e.ToString());
                }
            });

            progress.ProgressChanged -= Progress_ProgressChanged;
            InProgress = false;

        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            Progress = e;
        }


        protected override void DisposeManagedResources()
        {
            Cancel();
            base.DisposeManagedResources();
        }
    }
}