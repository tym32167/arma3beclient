using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.SyncModule.SyncCore
{
    public class BackendClient
    {
        private readonly SyncCredentials _credentials;
        private readonly IHttpGenericClient _client;


        public BackendClient(SyncCredentials credentials, IHttpGenericClient client)
        {
            _credentials = credentials;
            _client = client;
        }

        public async Task<PlayerSyncResponse> GetPlayers(int offset, int count)
        {
            var uri = $"{_credentials.Endpoint}/api/sync/players?offset={offset}&count={count}";
            _client.SetHeader("Authorization", $"Bearer {await GetToken()}");
            return await _client.GetAsync<PlayerSyncResponse>(uri);
        }

        public async Task PostPlayers(PlayerSyncRequest request)
        {
            var uri = $"{_credentials.Endpoint}/api/sync/players";
            _client.SetHeader("Authorization", $"Bearer {await GetToken()}");
            await _client.PostAsync<PlayerSyncRequest, string>(uri, request);
        }

        private string _token;
        public async Task<string> GetToken()
        {
            //if (!string.IsNullOrEmpty(_token))
            //{
            //    var handler = new JwtSecurityTokenHandler();
            //    var tokenInfo = handler.ReadJwtToken(_token);
            //    var tokenDate = new DateTime(tokenInfo.ValidTo.Ticks, DateTimeKind.Utc);
            //    var targetDate = DateTime.UtcNow.AddMinutes(-10);
            //    if (tokenDate > targetDate)
            //        _token = null;
            //}

            if (string.IsNullOrEmpty(_token))
            {
                var uri = $"{_credentials.Endpoint}/api/account/auth";
                var response = await _client.PostAsync<AuthRequest, AuthResponse>(uri, new AuthRequest()
                {
                    Username = _credentials.Login,
                    Password = _credentials.Password
                });

                _token = response.Token;
            }

            return _token;
        }
    }
}