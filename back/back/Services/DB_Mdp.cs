using System;
using System.Data;
using System.Security.Cryptography;
using back.InterneModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace back.Services
{
    public class DB_Mdp
    {
        //private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
        private GestionMdpContext context { init; get; }
        private const bool EST_MODE_PROD = true;

        private IConfiguration config;

        /// <summary>
        ///  A utiliser seulement pour le timer
        ///  Ne pas utiliser pour les APIs
        /// </summary>
        public DB_Mdp() { }

        public DB_Mdp(GestionMdpContext _context)
        {
            context = _context;
        }

        public DB_Mdp(GestionMdpContext _context, IConfiguration _config): this(_context)
        {
            config = _config;
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

        public async Task<List<MdpExport>> ListerMdpPartagerAvecMoi(int _idCompte)
        {
            List <dynamic> listeHashCle = new();

            List<MdpExport> listeRetour = new();

            using (SqlConnection con = new(config.GetConnectionString("ionos")))
            {
                await con.OpenAsync();
                var cmd = con.CreateCommand();

                cmd.CommandText = "SELECT mdp.id, login, mdp, mdp.titre, url, dateExpiration, description, mdp.idCompteCreateur " +
                                "FROM CompteGroupe cg " +
                                "JOIN Groupe g ON cg.idGroupe = g.id " +
                                "JOIN GroupeMdp gm ON gm.idGroupe = g.id " +
                                "JOIN MotDePasse mdp ON mdp.id = gm.idMdp " +
                                $"WHERE idCompte = @id";

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = _idCompte;
                await cmd.PrepareAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    string hashCleCompteActuel = context.Comptes.Where(c => c.Id == _idCompte).Select(c => c.HashCle).First();
                    listeHashCle.Add(new { hashCle = hashCleCompteActuel, idCompteCreateur = _idCompte });

                    AESprotection AESprotectionCompteActuel = new(hashCleCompteActuel);

                    while (await reader.ReadAsync())
                    {
                        string hashCle = "";

                        int idCompteCreateur = reader.GetInt32(7);
                        int index = listeHashCle.FindIndex(c => c.idCompteCreateur == idCompteCreateur);

                        if (index != -1)
                        {
                            hashCle = listeHashCle[index].hashCle;
                        }
                        else
                        {
                            hashCle = context.Comptes.Where(c => c.Id == idCompteCreateur).Select(c => c.HashCle).First();
                            listeHashCle.Add(new { hashCle = hashCle, idCompteCreateur = idCompteCreateur });
                        }

                        AESprotection aesProtection = new(hashCle);

                        string loginClaire = aesProtection.Dechiffrer(reader.GetString(1));
                        string mdpClaire = aesProtection.Dechiffrer(reader.GetString(2));
                        string titreClaire = aesProtection.Dechiffrer(reader.GetString(3));
                        string urlClaire = aesProtection.Dechiffrer(reader.GetString(4));
                        string descriptionClaire = aesProtection.Dechiffrer(reader.GetString(5));
                        string dateExpiClaire = aesProtection.Dechiffrer(reader.GetString(6));

                        listeRetour.Add(new MdpExport() {
                            Id = reader.GetInt32(0),
                            Login = AESprotectionCompteActuel.Chiffrer(loginClaire),
                            Mdp = AESprotectionCompteActuel.Chiffrer(mdpClaire),
                            Titre = AESprotectionCompteActuel.Chiffrer(titreClaire),
                            Url = AESprotectionCompteActuel.Chiffrer(urlClaire),
                            Description = AESprotectionCompteActuel.Chiffrer(descriptionClaire),
                            DateExpiration = AESprotectionCompteActuel.Chiffrer(dateExpiClaire)
                        });
                    }

                    await reader.CloseAsync();
                    await con.CloseAsync();
                }

                return listeRetour;
            }
        }

        /// <summary>
        /// A utiliser seulement par le timer
        /// </summary>
        public async Task EnvoyerMailMdpBientotExpirerAsync(string _connectionString, IConfiguration _config)
        {
            if (context is null)
                return;

            using(SqlConnection sqlCon = new (_connectionString))
            {
                await sqlCon.OpenAsync();

                SqlCommand cmd = sqlCon.CreateCommand();

                cmd.CommandText = "SELECT idCompteCreateur, mail, dateExpiration, hashCle, titre " +
                                  "FROM MotDePasse mdp JOIN Compte c ON mdp.idCompteCreateur = c.id " +
                                  "ORDER BY idCompteCreateur";

                await cmd.PrepareAsync();
                await cmd.ExecuteNonQueryAsync();

                using(var reader = cmd.ExecuteReader())
                {
                    int idCreateur = 0;

                    Mail mail;
                    AESprotection aes = new("azertyuiopazertyuiopazertyuiopaw");

                    while (reader.Read())
                    {
                        if(idCreateur != reader.GetInt32(0))
                        {
                            idCreateur = reader.GetInt32(0);
                            aes = new(reader.GetString(3));
                        }

                        DateTime dateExpiration = DateTime.Parse(aes.Dechiffrer(reader.GetString(2)));

                        if((dateExpiration - DateTime.Now).Days is 1)
                        {
                            string titre = aes.Dechiffrer(reader.GetString(4));

                            string message = "PasseBase bonjour, \n\n" +
                                             $"Votre mot de passe pour {titre} à la date du {dateExpiration.ToString("d")} va bientôt expiré \n\n" +
                                             $"N'oublier pas de le modifier en meme temps sur l'application en cliquant <a href='{(EST_MODE_PROD ? _config.GetValue<string>("mailProd:baseUrl") : _config.GetValue<string>("mail:baseUrl"))}'>ici</a> \n" +
                                             "A bientôt sur passeBase";

                            mail = new(reader.GetString(1), message, "PasseBase, mot de passe bientôt expirer", _config);
                            await mail.EnvoyerAsync();
                        }
                    }

                    await sqlCon.CloseAsync();
                    await reader.CloseAsync();
                }
            }
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
