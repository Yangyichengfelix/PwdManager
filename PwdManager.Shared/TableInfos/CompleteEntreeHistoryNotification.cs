using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.EntreeLogs;


namespace PwdManager.Shared.TableInfos
{
    public class CompleteEntreeHistoryNotification : BaseNotification
    {
        public EntreeLogNotificationData data { get; set; } = new EntreeLogNotificationData();
    }
}
