using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdManager.Shared.Dtos.Coffres
{
    public class CoffreUpdateDto
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        /// <summary>
        /// Titre de l'élément
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// description de l'élément
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// date de création
        /// </summary>
        public string Salt { get; set; } = string.Empty;
        /// <summary>
        /// Clé coffre
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// date de modification
        /// </summary>
        public DateTime? Modified { get; set; }
    }
}
