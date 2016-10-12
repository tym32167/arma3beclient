using Arma3BE.Client.Infrastructure.Models;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Arma3BE.Client.Modules.ToolsModule.ViewModels
{
    public class ExportViewModel : ViewModelBase
    {
        private readonly Exporter _exporter;
        private bool _isBisy;
        private string _selectedFile;

        public ExportViewModel(Exporter exporter)
        {
            _exporter = exporter;
            ExportCommand = new DelegateCommand(DoExport, CanDoExport);
            SelectFileCommand = new DelegateCommand(SelectFile);
        }

        private void SelectFile()
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "*.xml",
                Filter = "*.xml|*.xml",
                Title = "Select file to save players",
                FileName = $"Players{DateTime.Now:_yyyy_MM_dd}.xml",
            };

            var res = dlg.ShowDialog(Application.Current.MainWindow);

            if (res == true)
            {
                SelectedFile = dlg.FileName;
            }
        }

        private bool CanDoExport()
        {
            return string.IsNullOrEmpty(SelectedFile) == false;
        }

        private void DoExport()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            Task.Run(() =>
            {

                try
                {
                    IsBisy = true;
                    _exporter.Export(SelectedFile);

                    dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Export finished", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception e)
                {
                    dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, e.Message, "Export", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
                finally
                {
                    IsBisy = false;
                }
            });
        }



        public bool IsBisy
        {
            get { return _isBisy; }
            set
            {
                _isBisy = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
                ExportCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand SelectFileCommand { get; set; }
        public DelegateCommand ExportCommand { get; set; }
    }
}