using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.Entrees
{
    public class EntreeReadModel
    {
        public int Id { get; set; }
        public string EncryptedLogin { get; set; } = "";
        public string EncryptedPwd { get; set; } = "";
        public string EncryptedUrl { get; set; } = "";
        public string Url { get; set; } = "";
        public string Login { get; set; } = "";
        public string Pwd { get; set; } = "";
        public string Icon { get; set; } = "";
        public bool visible { get; set; } = false;
    }
}
