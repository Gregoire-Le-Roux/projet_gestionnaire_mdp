namespace back.Services
{
    public class DB_Mdp
    {
        private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
        private GestionMdpContext context { init; get; }

        public DB_Mdp(GestionMdpContext _context)
        {
            context = _context;
        }

        public MdpExport[] ListerMesMdp(int _id)
        {
            MdpExport[] liste = (from mdp in context.MotDePasses
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

        public MdpExport[] ListerMdpPartagerAvecMoi(int _id)
        {
            return new MdpExport[0];
        }

        public int Ajouter(MotDePasse _mdp)
        {
            context.MotDePasses.Add(_mdp);
            context.SaveChanges();

            int id = context.MotDePasses.OrderByDescending(m => m.Id).Select(m => m.Id).First();

            return id;
        }

        public void Modifier(MotDePasse _mdp)
        {
            context.MotDePasses.Update(_mdp);
            context.SaveChanges();
        }
    }
}
