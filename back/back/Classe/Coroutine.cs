using System.Timers;

namespace back.Classe
{
    public static class Coroutine
    {
        private static DB_Mdp dbMdp = new();
        private static string connectionString = "";

        static System.Timers.Timer timer = new()
        {
            Interval = 86400000,
            AutoReset = true
        };

        public static void Init(string _connectionString)
        {
            connectionString = _connectionString;
            timer.Elapsed += Test;
            timer.Start();
        }

        private static async void Test(object _obj, ElapsedEventArgs e)
        {
            await dbMdp.EnvoyerMailMdpBientotExpirerAsync(connectionString);

            Console.WriteLine("fini");
        }
    }
}
