using Microsoft.AspNetCore.Mvc;
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
            dbCompte = new(_context, _config);
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


        [HttpGet("mdpOublier/{mail}")]
        public async Task<string> MdpOublier(string mail)
        {
            if (!await dbCompte.ExisteAsync(mail))
                return JsonConvert.SerializeObject("Cette adresse mail n'existe pas");

            int idCompte = await dbCompte.GetId(mail);

            Token token = new(config);
            string jwt = token.Generer(idCompte);

            string message = "Bonjour \n\n" +
                            "Une demande de mot de passe oublié à été demandée \n" +
                            "Si cela ne vient pas de votre part ignorer simplement ce mail \n\n" +
                            "Sinon merci de cliquer sur le lien ci-dessous pour procéder à la modification de votre mot de passe \n" +
                            $"<a href='http://localhost:4200/#/nouveau-mot-de-passe/{jwt}'>Mot de passe oublié</a> \n\n" +
                            "A bientôt sur passeBase !";

            Mail mail1 = new(mail, message, "PasseBase, mot de passe oublié");
            mail1.EnvoyerAsync();

            return JsonConvert.SerializeObject("Un mail vous à été envoyé pour votre mot de passe oublié");
        }

        [HttpGet("existe/{mail}")]
        public async Task<string> Existe([FromRoute] string mail)
        {
            bool existe = await dbCompte.ExisteAsync(mail);

            return JsonConvert.SerializeObject(existe);
        }

        [HttpPost("demanderInscription")]
        public async Task<string> Ajouter([FromBody] CompteImport _compte)
        {
            AESprotection aes = new(CLE_SECRETE);
            string mailSecu = Protection.XSS(aes.Dechiffrer(_compte.Mail));
            string mdpSecu = aes.Dechiffrer(_compte.Mdp);

            if (await dbCompte.ExisteAsync(mailSecu))
                return JsonConvert.SerializeObject("Cette adresse mail est déjà utilisée");

            CompteImport compte = new()
            {
                Nom = _compte.Nom,
                Prenom = _compte.Prenom,
                Mail = _compte.Mail,
                Mdp = BC.HashPassword(mdpSecu)
            };

            Token token = new(config);
            string jwt = token.Generer(compte);

            Mail mail = new(mailSecu, jwt)
            {
                Nom = aes.Dechiffrer(compte.Nom),
                Prenom = aes.Dechiffrer(compte.Prenom)
            };

            mail.EnvoyerAsync();

            return JsonConvert.SerializeObject("Un mail vous a été envoyé pour confirmer votre inscription");
        }

        [Authorize]
        [HttpPost("inscription")]
        public async Task<string> Inscription(CompteImport _compte)
        {
            AESprotection aes = new(CLE_SECRETE);
            string nomSecu = Protection.XSS(aes.Dechiffrer(_compte.Nom));
            string prenomSecu = Protection.XSS(aes.Dechiffrer(_compte.Prenom));
            string mailSecu = Protection.XSS(aes.Dechiffrer(_compte.Mail));

            if (await dbCompte.ExisteAsync(mailSecu))
                return JsonConvert.SerializeObject("Cette adresse mail est déjà utilisée");

            string cleAES = AESprotection.CreerCleChiffrement(nomSecu, prenomSecu, mailSecu);

            AESprotection aESprotection = new(cleAES);

            Compte compte = new()
            {
                Nom = aESprotection.Chiffrer(nomSecu),
                Prenom = aESprotection.Chiffrer(prenomSecu),
                Mail = mailSecu,
                Mdp = _compte.Mdp,
                HashCle = cleAES
            };

            int id = await dbCompte.AjouterAsync(compte);

            Token token = new(config);
            string Jwt = token.Generer(id);

            return JsonConvert.SerializeObject(new { Id = id, HashCle = cleAES, Jwt = Jwt });
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
