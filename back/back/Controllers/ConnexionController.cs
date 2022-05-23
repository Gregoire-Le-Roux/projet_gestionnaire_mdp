using back.InterneModels;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConnexionController : Controller
    {
        private DB_Compte dbCompte;
        private IConfiguration config;
        private const string CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

        public ConnexionController(GestionMdpContext _context, IConfiguration _config)
        {
            dbCompte = new(_context);
        }

        /// <summary>
        ///     Tout doit etre envoyé chiffré
        /// </summary>
        /// <param name="_log"></param>
        /// <returns></returns>
        [HttpPost("Connexion")]
        public async Task<string> Connexion([FromBody] LogImport _log)
        {
            try
            {
                AESprotection? aes = new(CLE_SECRETE);

                string login = aes.Dechiffrer(_log.login);
                string mdp = aes.Dechiffrer(_log.mdp);

                aes = null;

                if (await dbCompte.ExisteAsync(login))
                {
                    InterneCompte compte = await dbCompte.InfoConnexionAsync(login);

                    if (BC.Verify(mdp, compte.HashMdp))
                    {
                        CompteExport infoCompte = await dbCompte.CompteAsync(compte.Id);

                        aes = new(infoCompte.HashCle);

                        infoCompte.Mail = aes.Chiffrer(infoCompte.Mail);
                        infoCompte.HashCle = Convert.ToBase64String(Encoding.UTF8.GetBytes(infoCompte.HashCle));

                        Token token = new(config);
                        infoCompte.Jwt = token.Generer(infoCompte.Id);

                        return JsonConvert.SerializeObject(infoCompte);
                    }
                }

                return JsonConvert.SerializeObject("Le login ou le mot de passe est incorrect");
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
