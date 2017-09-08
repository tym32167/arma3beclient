using System.Reflection;
using System.Windows;

namespace Arma3BEClient
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var title = $"TEHGAM.COM - Arma 3 BattlEye Tool v.{version.Major}.{version.Minor}.{version.Build}";
            Title = title;
        }
    }
}
