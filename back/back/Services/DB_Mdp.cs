using System.Data;
using back.InterneModels;
using Microsoft.Data.SqlClient;

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

        public async Task<bool> SupprimerAsync(int _idMdp, int _idCompte, string _connexionString)
        {
            bool estSupp = false;

            await Task.Run(() =>
            {
                using(SqlConnection sqlCon = new(_connexionString))
                {
                    sqlCon.Open();

                    SqlCommand cmd = sqlCon.CreateCommand();
                    cmd.Parameters.Add("@idMdp", SqlDbType.Int).Value = _idMdp;
                    cmd.Parameters.Add("@idCompte", SqlDbType.Int).Value = _idCompte;

                    cmd.CommandText = "SELECT COUNT(*) FROM MotDePasse WHERE id = @idMdp AND idCompteCreateur = @idCompte";

                    cmd.Prepare();

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    Console.WriteLine("---------------------------------");
                    Console.WriteLine(count);

                    if (count == 0)
                    {
                        estSupp = false;
                        return;
                    }

                    cmd.CommandText = "DELETE FROM GroupeMdp WHERE idMdp = @idMdp";

                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DELETE FROM MotDePasse WHERE id = @idMdp";

                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    sqlCon.Close();

                    estSupp = true;
                }
            });

            return estSupp;
        }
    }
}
