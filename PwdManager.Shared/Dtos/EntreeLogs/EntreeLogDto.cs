using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.EntreeLogs
{
    public class EntreeLogDto
    {
        public int EntreeHistoryId { get; set; }
        public DateTime DateOperation { get; set; }
        public string Operation { get; set; }=string.Empty;
 
        public string UserId { get; set; } = string.Empty;
        public string AzureId { get; set; } = string.Empty;


        public int EntreeId { get; set; }
       // public Entree Entree { get; set; }
        public string? LastVersion { get; set; }
    }
}
