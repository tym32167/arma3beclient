using System.Threading.Tasks;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.Core
{
    public interface IReasonRepository
    {
        void Dispose();
        Task<string[]> GetBadNicknamesAsync();
        Task<string[]> GetBanReasonsAsync();
        Task<BanTime[]> GetBanTimesAsync();
        Task<string[]> GetImportantWordsAsync();
        Task<string[]> GetKickReasonsAsync();
        Task UpdateBadNicknames(string[] badNicknames);
        Task UpdateBanReasons(string[] banReasons);
        Task UpdateBanTimes(BanTime[] banTimes);
        Task UpdateImportantWords(string[] importantWords);
        Task UpdateKickReasons(string[] kickReasons);
    }
}