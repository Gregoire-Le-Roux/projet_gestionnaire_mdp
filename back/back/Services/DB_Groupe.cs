namespace back.Services
{
    public static class DB_Groupe
    {
        public static GestionMdpContext context;
        public static IConfiguration? Configuration;

        public static IQueryable ListerLesMiens(int _idCompte)
        {
            DB_Compte.context = context;
            string hashCle = DB_Compte.GetHashCle(_idCompte);

            var liste = from groupe in context.Groupes
                        where groupe.IdCompteCreateur.Equals(_idCompte)
                        select new
                        {
                            groupe.Id,
                            groupe.Titre,
                            listeMdp = groupe.IdMdps.Select(m =>
                                new
                                {
                                    m.Id,
                                    m.Login,
                                    m.Mdp,
                                    m.Url,
                                    m.Titre,
                                    m.DateExpiration
                                }),

                            listeCompte = ListerCompteGroupeAvecMonHash(groupe.IdComptes).ToList()
                        };

            return liste;
        }

        public static int Ajouter(Groupe _groupe)
        {
            context.Groupes.Add(_groupe);
            context.SaveChanges();

            int id = context.Groupes.Where(g => g.IdCompteCreateur == _groupe.IdCompteCreateur)
                                    .OrderByDescending(g => g.Id).Select(g => g.Id).First();

            return id;
        }

        public static void AjouterCompteGroupe(int[] _listeIdCompte, int _idGroupe)
        {
            List<int> listeIdCompte = new();

            foreach (var item in _listeIdCompte)
            {

            }
        }

        private static List<Compte> ListerCompteGroupeAvecMonHash(ICollection<Compte> _listeCompte)
        {
            List<Compte> listeRetour = new();

            foreach (Compte compte in _listeCompte)
            {
                AESprotection aes = new(compte.HashCle);

                listeRetour.Add(
                    new Compte 
                    { 
                        Nom = aes.Chiffrer(aes.Dechiffrer(compte.Nom)),
                        Prenom = aes.Chiffrer(aes.Dechiffrer(compte.Prenom)),
                        Mail = aes.Chiffrer(aes.Dechiffrer(compte.Mail))
                    });
            }

            return listeRetour;
        }
    }
}
