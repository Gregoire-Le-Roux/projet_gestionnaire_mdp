﻿using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompteController : Controller
    {
        private readonly DB_Compte dbCompte;
        private readonly Token tokenNoConfig;
        private IConfiguration config;
        private const string CLE_SECRETE = "qrNm9BJjJ729A2Qi2vbr28M99hHhPW2p";

        public CompteController(GestionMdpContext _context, IConfiguration _config)
        {
            dbCompte = new(_context);
            tokenNoConfig = new();
            config = _config;
        }

        [Authorize]
        [HttpGet("monCompte")]
        public async Task<string> Compte()
        {
            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int id = tokenNoConfig.GetIdCompte(token);

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

        [HttpPost("inscription")]
        public async Task<string> Ajouter([FromBody] CompteImport _compte)
        {
            try
            {
                AESprotection aes = new(CLE_SECRETE);
                string nomSecu = Protection.XSS(_compte.Nom);
                string prenomSecu = Protection.XSS(_compte.Prenom);
                string mailSecu = Protection.XSS(aes.Dechiffrer(_compte.Mail));
                string mdpSecu = aes.Dechiffrer(_compte.Mdp);

                Console.WriteLine("-------------------------------");
                Console.WriteLine(mdpSecu);

                if (await dbCompte.ExisteAsync(mailSecu))
                    return JsonConvert.SerializeObject("Cette adresse mail est déjà utilisée");

                string cleAES = AESprotection.CreerCleChiffrement(nomSecu, prenomSecu, mailSecu);

                AESprotection aESprotection = new(cleAES);

                Compte compte = new()
                {
                    Nom = aESprotection.Chiffrer(nomSecu),
                    Prenom = aESprotection.Chiffrer(prenomSecu),
                    Mail =  mailSecu,
                    Mdp = BC.HashPassword(mdpSecu),
                    HashCle = cleAES
                };

                int id = await dbCompte.AjouterAsync(compte);

                return JsonConvert.SerializeObject(new { Id = id, HashCle = cleAES });
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(0);
            }
        }

        /// <summary>
        ///     Suppprime le compte et toutes ses infos liées
        /// </summary>
        /// <response>True si tout est OK</response>
        [Authorize]
        [HttpDelete("supprimer")]
        public async Task<string> Supprimer()
        {
            try
            {
                // recupere le token et vire le bearer
                string token = HttpContext.Request.Headers.Authorization;
                int id = tokenNoConfig.GetIdCompte(token);

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
