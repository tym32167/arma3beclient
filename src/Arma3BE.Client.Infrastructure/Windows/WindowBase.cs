using Arma3BEClient.Libs.Tools;
using System.Windows;

namespace Arma3BE.Client.Infrastructure.Windows
{
    public abstract class WindowBase : Window
    {
        public WindowBase(ISettingsStoreSource settingsStoreSource)
        {
            this.Topmost = settingsStoreSource.GetSettingsStore().TopMost;
        }
    }
}