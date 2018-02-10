using Arma3BEClient.Libs.Core.Settings;

namespace Arma3BEClient.Libs.EF.Settings
{
    public class SettingsStoreSource : ISettingsStoreSource
    {
        public ISettingsStore GetSettingsStore()
        {
            return SettingsStore.Instance;
        }

        public ICustomSettingsStore GetCustomSettingsStore()
        {
            return CustomSettingsStore.Instance;
        }
    }
}