namespace Arma3BEClient.Libs.Core.Settings
{
    public interface ISettingsStoreSource
    {
        ISettingsStore GetSettingsStore();
        ICustomSettingsStore GetCustomSettingsStore();
    }
}