using System.Windows;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;

namespace Arma3BE.Client.Modules.DevTools.Windows
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : Window
    {
        public LogViewer()
        {
            InitializeComponent();

            foreach (var generateColumn in GridHelper.GenerateColumns<LoggerMessage>())
            {
                dg.Columns.Add(generateColumn);
            }
        }
    }
}
