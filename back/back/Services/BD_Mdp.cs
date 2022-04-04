namespace back.Services
{
    public static class BD_Mdp
    {
        private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
        public static GestionMdpContext context;

        public static MdpExport[] ListerMesMdp(int _id)
        {
            var liste = (from mdp in context.MotDePasses
                        where mdp.IdCompteCreateur == _id
                        select new MdpExport
                        {
                            Id = mdp.Id,
                            Titre = mdp.Titre,
                            Url = mdp.Url,
                            DateExpiration = mdp.DateExpiration,
                            Description = mdp.Description,
                            Login = mdp.Login,
                            Mdp = mdp.Mdp,
                            IdCompteCreateur = mdp.IdCompteCreateur
                        }).ToArray();

            return liste;
        }

        public static int Ajouter(MotDePasse _mdp)
        {
            context.MotDePasses.Add(_mdp);
            context.SaveChanges();

            int id = context.MotDePasses.OrderByDescending(m => m.Id).Select(m => m.Id).First();

            return id;
        }
    }
}
