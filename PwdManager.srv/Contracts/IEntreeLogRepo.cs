using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.EntreeLogs;
using PwdManager.srv.Models;

namespace PwdManager.srv.Contracts
{
    public interface IEntreeLogRepo
    {
        Task<bool> Save();
        Task<bool> Add(EntreeHistory log);
        Task<IList<EntreeHistory>> ReadEntreeLogByCoffreId(int coffreId);
        Task<IList<EntreeHistory>> ReadEntreeLogByEntreeId(int entreeId);
        Task<IList<EntreeHistory>> ReadEntreeLogByUserId(string azureId);
        Task<IList<EntreeHistory>> ReadEntreeLogWithTimeRange(DateTime start, DateTime end);
        Task<IEnumerable<EntreeLogNotificationData>> ReadEntreeLogWithTimeRangeYield(DateTime start, DateTime end, string azureId);

    }
}
