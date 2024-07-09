using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PwdManager.Shared;

namespace PwdManager.Shared.Data
{
    public class CoffreLog
    {
        [Key]
     
        public int CoffreLogId { get; set; }
        public DateTime DateOperation { get; set; }
        public Operation Operation { get; set; }
        [ForeignKey("ApiUser")]
        public required string UserId { get; set; }
        public  ApiUser ApiUser { get; set; } 
        public string CoffreName { get; set; }=string.Empty;
        public int? CoffreId { get; set; }

        public  Coffre? Coffre { get; set; } 
    }
}
