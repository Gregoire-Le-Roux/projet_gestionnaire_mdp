using System;
using System.Collections.Generic;

namespace back.Models
{
    public partial class Compte
    {
        public Compte()
        {
            Groupes = new HashSet<Groupe>();
            MotDePasses = new HashSet<MotDePasse>();
            IdCompteCarnets = new HashSet<Compte>();
            IdComptes = new HashSet<Compte>();
            IdGroupes = new HashSet<Groupe>();
        }

        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string Mdp { get; set; } = null!;
        public string HashCle { get; set; } = null!;

        public virtual ICollection<Groupe> Groupes { get; set; }
        public virtual ICollection<MotDePasse> MotDePasses { get; set; }

        public virtual ICollection<Compte> IdCompteCarnets { get; set; }
        public virtual ICollection<Compte> IdComptes { get; set; }
        public virtual ICollection<Groupe> IdGroupes { get; set; }
    }
}
