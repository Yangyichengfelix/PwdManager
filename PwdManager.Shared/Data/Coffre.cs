using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Data
{
    public class Coffre
    {
        /// <summary>
        /// Clé primaire de la table Coffre
        /// </summary>
        [Key]

        public int Id { get; set; }
        /// <summary>
        /// Mot de passe chiffré
        /// </summary>


        public required string PasswordHash { get; set; }
        /// <summary>
        /// Titre de l'élément
        /// </summary>


        public string Title { get; set; } = "";
        /// <summary>
        /// description de l'élément
        /// </summary>


        public string Description { get; set; } = "";
        /// <summary>
        /// date de création
        /// </summary>

        public required string Salt { get; set; }
        /// <summary>
        /// Clé coffre
        /// </summary>


        public DateTime? Created { get; set; }
        /// <summary>
        /// date de modification
        /// </summary>


        public DateTime? Modified { get; set; }
        /// <summary>
        /// navigation à la table UserCoffre
        /// </summary>
        public virtual IList<ApiUserCoffre> ApiUserCoffres { get; set; }=new List<ApiUserCoffre>();
        public virtual IList<Entree> Entrees { get; set; } =new List<Entree>(); 
        public virtual IList<CoffreLog> CoffreLogs { get; set; } = new List<CoffreLog>();
        public virtual IList<EntreeHistory> EntreeHistories { get; set; } = new List<EntreeHistory>();



    }
}
