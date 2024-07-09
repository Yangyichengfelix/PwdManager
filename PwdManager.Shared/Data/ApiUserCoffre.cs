using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PwdManager.Shared;

namespace PwdManager.Shared.Data
{
    public class ApiUserCoffre
    {

        /// <summary>
        ///coffreId, clé primaire composée avec UserId
        /// </summary>
        [ForeignKey("Coffre")]

        public  int CoffreId { get; set; }
        /// <summary>
        ///userId, clé primaire composée avec coffreId
        /// </summary>
        [Required]
        [ForeignKey("ApiUser")]
        public required string UserId { get; set; } 
        /// <summary>
        /// droit d'accès à la coffre
        /// </summary>
        public Access Access { get; set; }
        /// <summary>
        /// navigation à la table Coffre
        /// </summary>
        public Coffre? Coffre { get; set; }
        /// <summary>
        /// navigation à la table ApiUser
        /// </summary>
        public  ApiUser? ApiUser { get; set; }

    }
}
