namespace back.Services
{
    public static class BD_Mdp
    {
        private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
        public static GestionMdpContext context;

        public static int Ajouter(MotDePasse _mdp)
        {
            context.MotDePasses.Add(_mdp);
            context.SaveChanges();

            int id = context.MotDePasses.OrderByDescending(m => m.Id).Select(m => m.Id).First();

            return id;
        }
    }
}
