using System.Data;
using Microsoft.Data.SqlClient;

namespace back.Services
{
    public class DB_Groupe
    {
        public GestionMdpContext context;
        public IConfiguration configuration;

        public DB_Groupe(GestionMdpContext _context, IConfiguration _config)
        {
            context = _context;
            configuration = _config;
        }

        public async Task<IQueryable> ListerLesMiensAsync(int _idCompte)
        {
            IQueryable? liste = null;

            await Task.Run(async () =>
            {
                DB_Compte dbCompte = new(context);

                string hashCle = await dbCompte.GetHashCleAsync(_idCompte);

                liste = from groupe in context.Groupes
                        where groupe.IdCompteCreateur.Equals(_idCompte)
                        select new
                        {
                            groupe.Id,
                            groupe.Titre,
                            nbCompte = groupe.IdComptes.Count,
                            listeMdp = groupe.IdMdps.Select(m =>
                                new
                                {
                                    m.Id,
                                    m.Login,
                                    m.Mdp,
                                    m.Url,
                                    m.Titre,
                                    m.DateExpiration
                                })
                        };
            });


            return liste;
        }

        public async Task<List<int>> ListerIdCompteAsync(List<string> _listeMail)
        {
            List<int> listeMailCompte = new();

            string listeMailString = "";

            for (int i = 0; i < _listeMail.Count; i++)
            {
                string mail = _listeMail[i];

                listeMailString += $"'{mail}'";

                if (i < _listeMail.Count - 1)
                    listeMailString += ',';
            }

            using (SqlConnection sqlCon = new(configuration.GetConnectionString("pcPortable")))
            {
                sqlCon.Open();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"SELECT id FROM compte WHERE mail IN({listeMailString})";

                //cmd.Parameters.Add("@liste", SqlDbType.VarChar, listeMailString.Length).Value = listeMailString;
                await cmd.PrepareAsync();

                await cmd.ExecuteNonQueryAsync();

                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        listeMailCompte.Add(reader.GetInt32(0));
                    }

                    await reader.CloseAsync();
                    await sqlCon.CloseAsync();
                }

                return listeMailCompte;
            }
        }

        public async Task<int> AjouterAsync(Groupe _groupe)
        {
            await context.Groupes.AddAsync(_groupe);
            await context.SaveChangesAsync(true);

            int id = context.Groupes.Where(g => g.IdCompteCreateur == _groupe.IdCompteCreateur)
                                    .OrderByDescending(g => g.Id).Select(g => g.Id).First();

            return id;
        }

        public async Task AjouterCompteGroupeAsync(List<int> _listeIdCompte, int _idGroupe)
        {
            string valueInsert = "";

            for (int i = 0; i < _listeIdCompte.Count; i++)
            {
                int idCompte = _listeIdCompte[i];

                valueInsert += $"({_idGroupe}, {idCompte})";

                if (i < _listeIdCompte.Count - 1)
                    valueInsert += ",";
            }

            using (SqlConnection sqlCon = new(configuration.GetConnectionString("pcPortable")))
            {
                await sqlCon.OpenAsync();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"INSERT INTO CompteGroupe (idGroupe, idCompte) VALUES {valueInsert}";

                await cmd.PrepareAsync();

                await cmd.ExecuteNonQueryAsync();

                await sqlCon.CloseAsync();
            }
        }

        public async Task AjouterMdpGroupeAsync(int _idGroupe, int _idMdp)
        {
            using(SqlConnection sqlCon = new(configuration.GetConnectionString("pcPortable")))
            {
                await sqlCon.OpenAsync();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = "INSERT INTO GroupeMdp (idGroupe, idMdp) VALUES (@idGrp, @idMdp)";

                cmd.Parameters.Add("@idGrp", SqlDbType.Int).Value = _idGroupe;
                cmd.Parameters.Add("@idMdp", SqlDbType.Int).Value = _idMdp;

                await cmd.PrepareAsync();

                await cmd.ExecuteNonQueryAsync();

                await sqlCon.CloseAsync();
            }
        }

        private async Task<List<Compte>> ListerCompteGroupeAvecMonHash(ICollection<Compte> _listeCompte)
        {
            List<Compte> listeRetour = new();

            await Task.Run(() =>
            {
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
            });

            return listeRetour;
        }
    }
}
