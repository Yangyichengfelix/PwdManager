using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.UserCoffres
{
    public class UserCoffreDto
    {
        public int CoffreId { get; set; }

        public required string UserId { get; set; }

        public Access Access { get; set; }

   
    }
    public class AzureCoffreDto
    {
        public int CoffreId { get; set; }

        public required string AzureId { get; set; }

    }

    public class AzureCoffreAccessDto
    {
        public int CoffreId { get; set; }

        public required string AzureId { get; set; }
        public Access Access { get; set; }


    }
    public class AzureMultipleCoffresDto
    {
        public List<int> CoffreId { get; set; }= new List<int>();

        public required string AzureId { get; set; }

    }
    public class UserCoffreReadOnlyDto
    {
        public int CoffreId { get; set; }
        public int CoffreTitle { get; set; }


        public required string UserId { get; set; }
        public string? AzureId { get; set; }

        public Access Access { get; set; }




    }
}
