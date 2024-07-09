using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.Entrees
{
    public class EntreeDto
    {
        public int Id { get; set; }
        public string EncryptedLogin { get; set; } = "";
        public string EncryptedPwd { get; set; } = "";
        public string? EncryptedURL { get; set; }
        public string? Icon { get; set; }

        public string IVLogin { get; set; } = "";

        public string IVPwd { get; set; } = "";
        public string? IVUrl { get; set; }
        public string TagLogin { get; set; } = "";
        public string TagPwd { get; set; } = "";
        public string? TagUrl { get; set; }
        public int CoffreId { get; set; }
    }
    public class EntreeCreateDto
    {
        public string EncryptedLogin { get; set; } = "";
        public string EncryptedPwd { get; set; } = "";
        public string? EncryptedURL { get; set; }
        public string IVLogin { get; set; } = "";
        public string? Icon { get; set; }

        public string IVPwd { get; set; } = "";
        public string? IVUrl { get; set; }
        public string TagLogin { get; set; } = "";
        public string TagPwd { get; set; } = "";
        public string? TagUrl { get; set; }

        public int CoffreId { get; set; }
    }
    public class EntreeReadOnlyDto
    {
        public int Id { get; set; }
        public string EncryptedLogin { get; set; } = "";
        public string EncryptedPwd { get; set; } = "";
        public string? EncryptedURL { get; set; }
        public string? Icon { get; set; }

        public string IVLogin { get; set; } = "";

        public string IVPwd { get; set; } = "";
        public string? IVUrl { get; set; }
        public string TagLogin { get; set; } = "";
        public string TagPwd { get; set; } = "";
        public string? TagUrl { get; set; }

        public int CoffreId { get; set; }
        public string CoffreTitle { get; set; } = "";

    }
}
