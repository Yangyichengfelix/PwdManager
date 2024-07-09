using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.EntreeLogs
{
    public class EntreeLogNotificationData
    {
        public int? EntreeId { get; set; }
        public int? CoffreId { get; set; }
        public int EntreeHistoryId { get; set; }


        public string AzureId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime DateOperation { get; set; }
        public string CoffreTitle { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string EntreeName { get; set; } = string.Empty;

    }
}
