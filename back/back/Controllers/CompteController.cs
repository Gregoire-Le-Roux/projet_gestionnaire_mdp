using Microsoft.AspNetCore.Mvc;
using back.ImportModels;
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

        [HttpPost("ajouter")]
        public async Task<string> Ajouter([FromBody] CompteImport _compte)
        {
            try
            {
                string nomSecu = Protection.XSS(_compte.Nom);
                string prenomSecu = Protection.XSS(_compte.Prenom);
                string mailSecu = Protection.XSS(_compte.Mail);

                string cleAES = AESprotection.CreerCleChiffrement(nomSecu, prenomSecu, mailSecu);

                Compte compte = new()
                {
                    Nom = nomSecu,
                    Prenom = prenomSecu,
                    Mail = mailSecu,
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
