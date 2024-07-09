using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.CoffreLogs
{
    public class CoffreLogNotificationData
    {
        public int CoffreId { get; set; }

        public string? CoffreTitle { get; set; }
        public string? CoffreDescription { get; set; }
        public string? AzureId { get; set; }
        public string? UserId { get; set; }
        public DateTime DateOperation { get; set; }
        public string? Operation { get; set; }
        public string? CoffreName { get; set; }
    }
}
