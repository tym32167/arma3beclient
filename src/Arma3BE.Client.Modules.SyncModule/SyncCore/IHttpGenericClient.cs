using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.SyncModule.SyncCore
{
    public interface IHttpGenericClient
    {
        void SetHeader(string name, string value);
        Task<T> GetAsync<T>(string uri);
        Task<TK> PostAsync<T, TK>(string uri, T payload);
    }
}