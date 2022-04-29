using System.Data;
using Microsoft.Data.SqlClient;
using back.InterneModels;

namespace back.Services
{
    public class DB_Groupe
    {
        private GestionMdpContext context;
        private IConfiguration configuration;

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
                            NbCompte = groupe.IdComptes.Count,
                            NbMdp = groupe.IdMdps.Count,
                            ListeMdp = groupe.IdMdps.Select(m =>
                                new
                                {
                                    m.Id,
                                    m.Login,
                                    m.Mdp,
                                    m.Url,
                                    m.Titre,
                                    m.DateExpiration,
                                    m.Description
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

            using (SqlConnection sqlCon = new(configuration.GetConnectionString("ionos")))
            {
                sqlCon.Open();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"SELECT id FROM compte WHERE mail IN({listeMailString})";

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

        public async Task<InterneCompteGroupe> ListerCompteAsync(int _idGroupe)
        {
            InterneCompteGroupe liste = null;

            await Task.Run(() =>
            {
                liste = (from g in context.Groupes
                        where g.Id == _idGroupe
                        select new InterneCompteGroupe
                        {
                            ListeCompte = g.IdComptes.Select(c => new CompteGroupe
                            { Id = c.Id, Nom = c.Nom, Prenom = c.Prenom, Mail = c.Mail, HashCle = c.HashCle }).ToList()
                        }).First();
            });

            return liste;
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

            using (SqlConnection sqlCon = new(configuration.GetConnectionString("ionos")))
            {
                await sqlCon.OpenAsync();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"INSERT INTO CompteGroupe (idGroupe, idCompte) VALUES {valueInsert}";

                await cmd.PrepareAsync();

                await cmd.ExecuteNonQueryAsync();

                await sqlCon.CloseAsync();
            }
        }

        public async Task AjouterMdpGroupeAsync(int _idGroupe, int[] _listeIdMdp)
        {
            string valueGroupeMdp = "";

            for (int i = 0; i < _listeIdMdp.Length; i++)
            {
                int idMdp = _listeIdMdp[i];

                valueGroupeMdp += $"({_idGroupe}, {idMdp})";

                if (i < _listeIdMdp.Length - 1)
                    valueGroupeMdp += ",";
            }

            using(SqlConnection sqlCon = new(configuration.GetConnectionString("ionos")))
            {
                await sqlCon.OpenAsync();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"INSERT INTO GroupeMdp (idGroupe, idMdp) VALUES {valueGroupeMdp}";

                await cmd.PrepareAsync();

                await cmd.ExecuteNonQueryAsync();

                await sqlCon.CloseAsync();
            }
        }

        public async Task SupprimerAsync(int _idGroupe)
        {
            Groupe groupe = context.Groupes.First(g => g.Id == _idGroupe);

            context.Groupes.Remove(groupe);
            await context.SaveChangesAsync(true);
        }
        
        public async Task SupprimerCompteAsync(int[] _listeIdCompte, int _idGroupe)
        {
            string listeIdCompteString = string.Join(',', _listeIdCompte);

            using(SqlConnection sqlCon = new(configuration.GetConnectionString("ionos")))
            {
                await sqlCon.OpenAsync();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = $"DELETE FROM CompteGroupe WHERE idGroupe = @idGrp AND idCompte IN ({listeIdCompteString})";

                cmd.Parameters.Add("@idGrp", SqlDbType.Int).Value = _idGroupe;
                await cmd.PrepareAsync();

                await cmd.ExecuteReaderAsync();

                await sqlCon.CloseAsync();
            }
        }
    }
}
