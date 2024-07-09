using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwdManager.Shared.Data
{
    public class Entree
    {
        [Key]
        public int Id { get; set; }
        public required string EncryptedLogin { get; set; }
        public required string EncryptedPwd { get; set; }
        public string? EncryptedURL { get; set; }

        public required string IVLogin { get; set; }

        public required string IVPwd { get; set; }
        public string? Icon { get; set; }

        public string? IVUrl { get; set; }
        public required string TagLogin { get; set; }
        public required string TagPwd { get; set; }
        public string? TagUrl { get; set; }

        [ForeignKey("Coffre")]
        public int? CoffreId { get; set; }

        public Coffre? Coffre { get; set; } 

        public virtual IList<EntreeHistory>? EntreeHistories { get; set; }
    }
}
