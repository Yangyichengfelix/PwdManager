using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.srv.Models;

namespace PwdManager.srv.Contracts
{
    public interface ICoffreLogRepo
    {
        Task<bool> Save();
        Task<bool> Add(CoffreLog log);
        Task<IList<CoffreLog>> ReadCoffreLogByCoffreId(int coffreId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="azureId"></param>
        /// <returns></returns>
        Task<IList<CoffreLog>> ReadCoffreLogByUserId(string azureId);
        Task<IList<CoffreLog>> ReadCoffreLogWithTimeRange(DateTime start, DateTime end);
        Task<IEnumerable<CoffreLogNotificationData>> ReadCoffreLogWithTimeRangeYield(DateTime start, DateTime end, string azureId);
    }
}
