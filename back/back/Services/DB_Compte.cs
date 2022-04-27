
using System.Data;
using back.InterneModels;
using Microsoft.Data.SqlClient;

namespace back.Services;

public class DB_Compte
{
    private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
    private GestionMdpContext context;

    public DB_Compte(GestionMdpContext _context)
    {
        context = _context;
    }

    /// <summary>
    /// Recupere les infos pour l'authentification
    /// </summary>
    /// <param name="_mail"></param>
    /// <returns></returns>
    public async Task<InterneCompte> InfoConnexionAsync(string _mail)
    {
        InterneCompte? compte = null;

        await Task.Run(() =>
        {
            compte = (from c in context.Comptes
                      where c.Mail == _mail
                      select new InterneCompte
                      {
                          Id = c.Id,
                          HashMdp = c.Mdp
                      }).First();
        });


        return compte;
    }

    public async Task<CompteExport> CompteAsync(int _id)
    {
        CompteExport? compte = null;

        await Task.Run(() =>
        {
            compte = (from c in context.Comptes
                      where c.Id == _id
                      select new CompteExport
                      {
                          Id = c.Id,
                          Nom = c.Nom,
                          Prenom = c.Prenom,
                          Mail = c.Mail,
                          HashCle = c.HashCle
                      }).First();
        });

        return compte;
    }

    public async Task<int> AjouterAsync(Compte _compte)
    {
        await context.Comptes.AddAsync(_compte);
        await context.SaveChangesAsync();

        int id = context.Comptes.OrderByDescending(c => c.Id).Select(c => c.Id).First();

        return id;
    }

    public async Task SupprimerAsync(int _id)
    {
        // ouvre une connection a la bdd
        // using evite les fuite mémoire,
        // tout est en lecture seul,
        // a la fin du using tout est supprimé de la mémoire
        using(SqlConnection con = new(connectionString))
        {
            await con.OpenAsync();

            var cmd = con.CreateCommand();

            // supp le compte des groupes
            cmd.CommandText = "DELETE FROM CompteGroupe WHERE idCompte = @id";

            // parametre est dispo pour toute les requetes jusqu'a la fermeture de la liaison
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            await cmd.PrepareAsync();
            await cmd.ExecuteNonQueryAsync();

            // supp le compte des carnets d'adresse
            cmd.CommandText = "DELETE FROM CarnetAdresse WHERE idCompte = @id OR idCompteCarnet = @id";

            await cmd.PrepareAsync();
            await cmd.ExecuteNonQueryAsync();

            // supp les mdps partagés
            cmd.CommandText = "DELETE gMdp FROM GroupeMdp gMdp JOIN MotDePasse mdp ON gMdp.idMdp = mdp.idCompteCreateur WHERE idCompteCreateur = @id";

            await cmd.PrepareAsync();
            await cmd.ExecuteNonQueryAsync();

            // supp le compte
            cmd.CommandText = "DELETE FROM Compte WHERE id = @id";

            await cmd.PrepareAsync();
            await cmd.ExecuteNonQueryAsync();

            await con.CloseAsync();
        }
    }

    public async Task<bool> ExisteAsync(string _mail)
    {
        int retour = 0;

        await Task.Run(() => retour = context.Comptes.Count(c => c.Mail == _mail));

        return retour >= 1;
    }

    public async Task<string> GetHashCleAsync(int _id)
    {
        string hashCle = "";

        await Task.Run(() => hashCle = context.Comptes.Where(c => c.Id == _id).Select(c => c.HashCle).First());

        return hashCle;
    }

    public string[] Lister()
    {
        string[] tab = new string[2];

        using(SqlConnection connection = new(connectionString))
        {
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Compte";

            cmd.Prepare();

            cmd.ExecuteNonQuery();

            using(var reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    // si SELECT * => indiquer les nom des champs
                    // si SELECT nomChamps => indiquer les index des champs
                    Console.WriteLine(reader.GetString("nom"));
                }

                reader.Close();
                connection.Close();

                return tab;
            }
        }
    }
}

