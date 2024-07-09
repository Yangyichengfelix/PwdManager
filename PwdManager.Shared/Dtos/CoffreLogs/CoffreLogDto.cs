using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.CoffreLogs
{
    public class CoffreLogDto
    {
        public int CoffreLogId { get; set; }
        public DateTime DateOperation { get; set; }
        public string Operation { get; set; } = "";

        public string UserId { get; set; } = "";
        public string AzureId { get; set; } = "";

        public int CoffreId { get; set; }

        public string CoffreTitle { get; set; } = "";
        public string CoffreDescription { get; set; } = "";
    }
}
