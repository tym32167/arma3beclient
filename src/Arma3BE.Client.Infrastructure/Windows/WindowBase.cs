using Arma3BEClient.Libs.Tools;
using System.Windows;
using Arma3BEClient.Libs.Core.Settings;

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