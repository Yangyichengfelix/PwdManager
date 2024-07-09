
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PwdManager.Shared;
namespace PwdManager.Shared.Data
{
    [Index(nameof(EntreeId))]

    public class EntreeHistory

    {
        public string EncryptedLogin { get; set; }=string.Empty;
        public string EncryptedPwd { get; set; } = string.Empty;
        public string? EncryptedURL { get; set; } = string.Empty;

        public string IVLogin { get; set; } = string.Empty;

        public string IVPwd { get; set; } = string.Empty;

        public string? IVUrl { get; set; }
        public string TagLogin { get; set; } = string.Empty;
        public string TagPwd { get; set; } = string.Empty;
        public string? TagUrl { get; set; }


        public int? CoffreId { get; set; }

        public Coffre? Coffre { get; set; }
        [Key]
        public int EntreeHistoryId { get; set; }
        public DateTime DateOperation { get; set; }
        public Operation Operation { get; set; }
        [ForeignKey("ApiUser")]
        public required string UserId { get; set; }
        public ApiUser ApiUser { get; set; }  
        public string EntreeName { get; set; } = string.Empty;
        public int? EntreeId { get; set; }
        public virtual Entree? Entree { get; set; } 


    

    }
}
