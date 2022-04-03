using System;
using System.Collections.Generic;

namespace back.Models
{
    public partial class MotDePasse
    {
        public MotDePasse()
        {
            IdGroupes = new HashSet<Groupe>();
        }

        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Mdp { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string DateExpiration { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int IdCompteCreateur { get; set; }

        public virtual Compte IdCompteCreateurNavigation { get; set; } = null!;

        public virtual ICollection<Groupe> IdGroupes { get; set; }
    }
}
