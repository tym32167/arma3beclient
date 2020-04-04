namespace Arma3BE.Client.Modules.SyncModule
{
    public class SettingsKeys
    {
        private static string prefix = "SyncModuleSettings_";
        public static string UserNamePrefix => $"{prefix}UserName";
        public static string EndpointPrefix => $"{prefix}Endpoint";
    }
}