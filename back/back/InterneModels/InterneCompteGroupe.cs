namespace back.InterneModels
{
    public class InterneCompteGroupe
    {
        public List<CompteGroupe> ListeCompte = new List<CompteGroupe>();
    }

    public class CompteGroupe
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string HashCle { get; set; } = null!;
    }
}
