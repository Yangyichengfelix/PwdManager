

using PwdManager.Shared.Dtos.CoffreLogs;

namespace PwdManager.Shared.TableInfos
{
    public class CompleteCoffreLogNotification:BaseNotification
    {
        public CoffreLogNotificationData data { get; set; }=new CoffreLogNotificationData();
    }
}
