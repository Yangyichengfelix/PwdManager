using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos
{
    public class PasswordModel
    {
        public string? Pwd { get; set; }
        public string? PwdConfirm { get; set; }
    }
    public class EntreeCreateModel : PasswordModel
    {
        public string? Url { get; set; }
        public string? Login { get; set; }

    }
    public class EntreeUpdateModel : EntreeCreateModel
    {
        public int Id { get; set; }
    }
    public class CoffreUpdateModel : PasswordModel
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

    }
}
