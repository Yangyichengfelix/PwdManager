using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.Entrees;
using PwdManager.Shared.Dtos.UserCoffres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.Coffres
{
    public class CoffreLogReadOnlyDto:CoffreUpdateDto
    {

        public List<UserCoffreDto> ApiUserCoffres { get; set; } = new List<UserCoffreDto>();
        public List<CoffreLogDto> CoffreLogs { get; set; }= new List<CoffreLogDto>();   
    }
}
