using back.InterneModels;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConnexionController : Controller
    {
        private readonly GestionMdpContext context;
        private const string CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

        public ConnexionController(GestionMdpContext _context)
        {
            context = _context;
            DB_Compte.context = context;
        }

        /// <summary>
        ///     Tout doit etre envoyé chiffré
        /// </summary>
        /// <param name="_log"></param>
        /// <returns></returns>
        [HttpPost("Connexion")]
        public string Connexion([FromBody] LogImport _log)
        {
            try
            {
                AESprotection aes = new(CLE_SECRETE, CLE_SECRETE.Substring(0, 16));

                string login = aes.Dechiffrer(_log.login);
                string mdp = aes.Dechiffrer(_log.mdp);

                if (DB_Compte.Existe(login))
                {
                    InterneCompte compte = DB_Compte.InfoConnexion(login);

                    if (BC.Verify(mdp, compte.HashMdp))
                        return JsonConvert.SerializeObject(true);
                }

                return JsonConvert.SerializeObject(false);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
