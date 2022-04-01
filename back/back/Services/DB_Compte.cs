
using System.Data;
using Microsoft.Data.SqlClient;

namespace back.Services;

public static class DB_Compte
{
    public static GestionMdpContext context;

    public static int Ajouter(Compte _compte)
    {
        context.Comptes.Add(_compte);
        context.SaveChanges();

        int id = context.Comptes.OrderByDescending(c => c.Id).Select(c => c.Id).First();

        return id;
    }

    public static string[] Lister()
    {
        string[] tab = new string[2];

        using(SqlConnection connection = new("Data Source=DESKTOP-J5HTQCS\\SQLSERVER;Initial Catalog=GestionMdp;Integrated Security=True"))
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

