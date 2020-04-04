namespace Arma3BE.Client.Modules.SyncModule.SyncCore
{
    public class SyncCredentials
    {
        public string Endpoint { get; }
        public string Login { get; }
        public string Password { get; }

        public SyncCredentials(string endpoint, string login, string password)
        {
            Endpoint = endpoint;
            Login = login;
            Password = password;
        }
    }
}