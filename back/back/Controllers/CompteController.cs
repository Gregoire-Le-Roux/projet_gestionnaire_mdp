using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompteController : Controller
    {
        private GestionMdpContext mdpContext;

        public CompteController(GestionMdpContext _context)
        {
            mdpContext = _context;
            DB_Compte.context = _context;
        }

        [HttpGet("MonCompte/{id}")]
        public string Compte([FromRoute] int id)
        {
            // nicolas
            // peyrachon
            // nicolas@gmail.com
            // jeton!peche01

            var compte = DB_Compte.Compte(id);

            AESprotection aESprotection = new(compte.HashCle.Substring(0, 32), compte.HashCle.Substring(0, 16));

            compte.Nom = aESprotection.Dechiffrer(compte.Nom);
            compte.Prenom = aESprotection.Dechiffrer(compte.Prenom);
            compte.Mail = aESprotection.Dechiffrer(compte.Mail);

            return JsonConvert.SerializeObject(compte);
        }

        [HttpPost("ajouter")]
        public string Ajouter([FromBody] CompteImport _compte)
        {
            try
            {
                string nomSecu = Protection.XSS(_compte.Nom);
                string prenomSecu = Protection.XSS(_compte.Prenom);
                string mailSecu = Protection.XSS(_compte.Mail);

                if (DB_Compte.Existe(mailSecu))
                    return JsonConvert.SerializeObject("Cette adresse mail est déjà utilisée");

                string cleAES = AESprotection.CreerCleChiffrement(nomSecu, prenomSecu, mailSecu);

                AESprotection aESprotection = new(cleAES.Substring(0, 32), cleAES.Substring(0, 16));

                Compte compte = new()
                {
                    Nom = aESprotection.Chiffrer(nomSecu),
                    Prenom = aESprotection.Chiffrer(prenomSecu),
                    Mail =  aESprotection.Chiffrer(mailSecu),
                    Mdp = BC.HashPassword(_compte.Mdp),
                    HashCle = cleAES
                };

                int id = DB_Compte.Ajouter(compte);

                return JsonConvert.SerializeObject(id);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(0);
            }
        }

        /// <summary>
        ///     Suppprime le compte et toutes ses infos liées
        /// </summary>
        /// <param name="id"></param>
        /// <response>True si tout est OK</response>
        [HttpDelete("supprimer/{id}")]
        public async Task<string> Supprimer([FromRoute] int id)
        {
            try
            {
                DB_Compte.Supprimer(id);

                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
