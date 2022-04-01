using System;
using System.Collections.Generic;

namespace back.Models
{
    public partial class CarnetAdresse
    {
        public int Id { get; set; }
        public int IdCompte { get; set; }

        public virtual Compte IdCompteNavigation { get; set; } = null!;
    }
}
