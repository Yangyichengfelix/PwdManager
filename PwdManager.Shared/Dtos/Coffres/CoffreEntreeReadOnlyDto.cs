using PwdManager.Shared.Dtos.Entrees;
using PwdManager.Shared.Dtos.UserCoffres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.Coffres
{
    public class CoffreEntreeReadOnlyDto:CoffreUpdateDto
    {

        public List<UserCoffreDto> ApiUserCoffres { get; set; }=new List<UserCoffreDto>();
        public List<EntreeDto> Entrees { get; set; } = new List<EntreeDto>();   
    }
}
