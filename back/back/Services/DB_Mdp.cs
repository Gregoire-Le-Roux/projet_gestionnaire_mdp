namespace back.Services
{
    public class DB_Mdp
    {
        //private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
        private GestionMdpContext context { init; get; }

        public DB_Mdp(GestionMdpContext _context)
        {
            context = _context;
        }

        public async Task<MdpExport[]> ListerMesMdpAsync(int _id)
        {
            MdpExport[] liste = Array.Empty<MdpExport>();

            await Task.Run(() =>
            {
                liste = (from mdp in context.MotDePasses
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
            });

            return liste;
        }

        public MdpExport[] ListerMdpPartagerAvecMoi(int _id)
        {
            return new MdpExport[0];
        }

        public async Task<int> AjouterAsync(MotDePasse _mdp)
        {
            await context.MotDePasses.AddAsync(_mdp);
            await context.SaveChangesAsync(true);

            int id = context.MotDePasses.OrderByDescending(m => m.Id).Select(m => m.Id).First();

            return id;
        }

        public async Task ModifierAsync(MotDePasse _mdp)
        {
            context.MotDePasses.Update(_mdp);
            await context.SaveChangesAsync(true);
        }
    }
}
