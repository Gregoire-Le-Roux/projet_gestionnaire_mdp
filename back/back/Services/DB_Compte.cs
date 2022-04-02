
using System.Data;
using back.InterneModels;
using Microsoft.Data.SqlClient;

namespace back.Services;

public static class DB_Compte
{
    private readonly static string connectionString = "Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True";
    public static GestionMdpContext context;

    /// <summary>
    /// Recupere les infos pour l'authentification
    /// </summary>
    /// <param name="_mail"></param>
    /// <returns></returns>
    public static InterneCompte InfoConnexion(string _mail)
    {
        var compte = (from c in context.Comptes
                      where c.Mail == _mail
                      select new InterneCompte
                      {
                          Nom = c.Nom,
                          Prenom = c.Prenom,
                          Mail = c.Mail,
                          HashCle = c.HashCle,
                          HashMdp = c.Mdp
                      }).First();

        return compte;
    }

    public static CompteExport Compte(int _id)
    {
        var compte =  (from c in context.Comptes
                       where c.Id == _id
                       select new CompteExport
                       {
                           Id = c.Id,
                           Nom = c.Nom,
                           Prenom = c.Prenom,
                           Mail = c.Mail,
                           HashCle = c.HashCle
                       }).First();

        return compte;
    }

    public static int Ajouter(Compte _compte)
    {
        context.Comptes.Add(_compte);
        context.SaveChanges();

        int id = context.Comptes.OrderByDescending(c => c.Id).Select(c => c.Id).First();

        return id;
    }

    public static void Supprimer(int _id)
    {
        // ouvre une connection a la bdd
        // using evite les fuite mémoire,
        // tout est en lecture seul,
        // a la fin du using tout est supprimé de la mémoire
        using(SqlConnection con = new(connectionString))
        {
            con.Open();

            var cmd = con.CreateCommand();

            // supp le compte des groupes
            cmd.CommandText = "DELETE FROM CompteGroupe WHERE idCompte = @id";

            // parametre est dispo pour toute les requetes jusqu'a la fermeture de la liaison
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            cmd.Prepare();
            
            cmd.ExecuteNonQuery();

            // supp le compte des carnets d'adresse
            cmd.CommandText = "DELETE FROM CarnetAdresse WHERE idCompte = @id OR idCompteCarnet = @id";

            cmd.Prepare();
            cmd.ExecuteNonQuery();

            // supp les mdps partagés
            cmd.CommandText = "DELETE gMdp FROM GroupeMdp gMdp JOIN MotDePasse mdp ON gMdp.idMdp = mdp.idCompteCreateur WHERE idCompteCreateur = @id";

            cmd.Prepare();
            cmd.ExecuteNonQuery();

            // supp le compte
            cmd.CommandText = "DELETE FROM Compte WHERE id = @id";

            cmd.Prepare();
            cmd.ExecuteNonQuery();

            con.Close();
        }
    }

    public static bool Existe(string _mail)
    {
        int retour = context.Comptes.Count(c => c.Mail == _mail);

        return retour == 1;
    }

    public static string[] Lister()
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
                

                return tab;
            }
        }
    }
}

