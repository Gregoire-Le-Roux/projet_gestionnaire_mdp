using System;
using System.Collections.Generic;

namespace back.Models
{
    public partial class Compte
    {
        public Compte()
        {
            CarnetAdresses = new HashSet<CarnetAdresse>();
            Groupes = new HashSet<Groupe>();
            MotDePasses = new HashSet<MotDePasse>();
            IdGroupes = new HashSet<Groupe>();
        }

        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string Mdp { get; set; } = null!;

        public virtual ICollection<CarnetAdresse> CarnetAdresses { get; set; }
        public virtual ICollection<Groupe> Groupes { get; set; }
        public virtual ICollection<MotDePasse> MotDePasses { get; set; }

        public virtual ICollection<Groupe> IdGroupes { get; set; }
    }
}
