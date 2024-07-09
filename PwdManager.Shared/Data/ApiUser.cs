
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Data
{
    [DebuggerDisplay("User {UserId}, {AzureId}")]
    public class ApiUser
    {
        [Key]
        public required string UserId { get; set; }
        public required string AzureId { get; set; }
        /// <summary>
        /// navigation table UserCoffre
        /// </summary>
        public virtual IList<ApiUserCoffre> ApiUserCoffres { get; set; }= new List<ApiUserCoffre>();    
        public virtual IList<EntreeHistory> EntreeHistories { get; set; } = new List<EntreeHistory>();
        public virtual IList<CoffreLog> CoffreLogs { get; set; } = new List<CoffreLog>();
    }
}
