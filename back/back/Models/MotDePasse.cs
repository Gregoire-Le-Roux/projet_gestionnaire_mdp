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
        public int IdCompteCreateur { get; set; }

        public virtual Compte IdCompteCreateurNavigation { get; set; } = null!;

        public virtual ICollection<Groupe> IdGroupes { get; set; }
    }
}
