namespace Arma3BEClient.Libs.Core.Settings
{
    public interface ICustomSettingsStore
    {
        void Save(string key, string value);
        string Load(string key);
    }
}