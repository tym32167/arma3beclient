using System.Windows;

namespace Arma3BE.Frontend
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var boostrapper = new Bootstrapper();
            boostrapper.Run();
        }
    }
}