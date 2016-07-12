namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class ShowUserModel
    {
        public string UserGuid { get; }

        public ShowUserModel(string userGuid)
        {
            UserGuid = userGuid;
        }
    }
}