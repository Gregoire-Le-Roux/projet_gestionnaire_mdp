using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompteController : Controller
    {
        private readonly DB_Compte dbCompte;

        public CompteController(GestionMdpContext _context)
        {
            dbCompte = new(_context);
        }

        [HttpGet("MonCompte/{id}")]
        public async Task<string> Compte([FromRoute] int id)
        {
            var compte = await dbCompte.CompteAsync(id);

            AESprotection? aESprotection = new(compte.HashCle);

            compte.Nom = aESprotection.Dechiffrer(compte.Nom);
            compte.Prenom = aESprotection.Dechiffrer(compte.Prenom);

            return JsonConvert.SerializeObject(compte);
        }

        [HttpGet("existe/{mail}")]
        public async Task<string> Existe([FromRoute] string mail)
        {
            bool existe = await dbCompte.ExisteAsync(mail);

            return JsonConvert.SerializeObject(existe);
        }

        [HttpPost("ajouter")]
        public async Task<string> Ajouter([FromBody] CompteImport _compte)
        {
            try
            {
                string nomSecu = Protection.XSS(_compte.Nom);
                string prenomSecu = Protection.XSS(_compte.Prenom);
                string mailSecu = Protection.XSS(_compte.Mail);

                if (await dbCompte.ExisteAsync(mailSecu))
                    return JsonConvert.SerializeObject("Cette adresse mail est déjà utilisée");

                string cleAES = AESprotection.CreerCleChiffrement(nomSecu, prenomSecu, mailSecu);

                AESprotection aESprotection = new(cleAES);

                Compte compte = new()
                {
                    Nom = aESprotection.Chiffrer(nomSecu),
                    Prenom = aESprotection.Chiffrer(prenomSecu),
                    Mail =  mailSecu,
                    Mdp = BC.HashPassword(_compte.Mdp),
                    HashCle = cleAES
                };

                int id = await dbCompte.AjouterAsync(compte);

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
                await dbCompte.SupprimerAsync(id);

                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
