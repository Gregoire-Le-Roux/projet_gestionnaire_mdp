using System.Timers;

namespace back.Classe
{
    public static class Coroutine
    {
        private static DB_Mdp dbMdp = new();
        private static string connectionString = "";
        private static IConfiguration config;

        static System.Timers.Timer timer = new()
        {
            Interval = 86400000,
            AutoReset = true
        };

        public static void InitEnvoieMailAuto(string _connectionString, IConfiguration _config)
        {
            config = _config;
            connectionString = _connectionString;
            timer.Elapsed += Test;
            timer.Start();
        }

        private static async void Test(object _obj, ElapsedEventArgs e)
        {
            await dbMdp.EnvoyerMailMdpBientotExpirerAsync(connectionString, config);
        }
    }
}
