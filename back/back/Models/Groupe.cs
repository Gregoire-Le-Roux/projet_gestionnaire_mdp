using System;
using System.Collections.Generic;

namespace back.Models
{
    public partial class Groupe
    {
        public Groupe()
        {
            IdComptes = new HashSet<Compte>();
            IdMdps = new HashSet<MotDePasse>();
        }

        public int Id { get; set; }
        public int IdCompteCreateur { get; set; }
        public string Titre { get; set; } = null!;

        public virtual Compte IdCompteCreateurNavigation { get; set; } = null!;

        public virtual ICollection<Compte> IdComptes { get; set; }
        public virtual ICollection<MotDePasse> IdMdps { get; set; }
    }
}
