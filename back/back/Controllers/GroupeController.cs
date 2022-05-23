using Microsoft.AspNetCore.Mvc;
using back.ImportModels;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupeController : ControllerBase
    {
        private readonly DB_Groupe dbGroupe;
        private readonly Token tokenNoConfig;
        private readonly GestionMdpContext context;

        public GroupeController(GestionMdpContext _context, IConfiguration _config)
        {
            tokenNoConfig = new();
            dbGroupe = new (_context, _config);
            context = _context;
        }

        [Authorize]
        [HttpGet("lister")]
        public async Task<string> Lister()
        {
            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            var liste = await dbGroupe.ListerLesMiensAsync(idCompte);

            return JsonConvert.SerializeObject(liste);
        }

        [Authorize]
        [HttpGet("listerCompte/{idGroupe}")]
        public async Task<string> ListerMdpCompte([FromRoute] int idGroupe)
        {
            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            DB_Compte dbCompte = new(context);
            string hashCle = await dbCompte.GetHashCleAsync(idCompte);

            List<object> liste = new List<object>();

            InterneModels.InterneCompteGroupe compteGroupe = await dbGroupe.ListerCompteAsync(idGroupe);

            AESprotection aesCreateur = new(hashCle);

            // chiffre les infos avec le hashcle du propriétaire du groupe
            foreach (var compte in compteGroupe.ListeCompte)
            {
                AESprotection aes = new(compte.HashCle);

                compte.Prenom = aesCreateur.Chiffrer(aes.Dechiffrer(compte.Prenom));
                compte.Nom = aesCreateur.Chiffrer(aes.Dechiffrer(compte.Nom));
                compte.Mail = aesCreateur.Chiffrer(compte.Mail);

                liste.Add(new { Id = compte.Id, Prenom = compte.Prenom, Nom = compte.Nom, Mail = compte.Mail });
            }

            return JsonConvert.SerializeObject(liste);
        }

        [Authorize]
        [HttpPost("ajouter")]
        public async Task<string> Ajouter([FromBody] groupeImport _groupe)
        {

            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            List<string> listeMailSecuDechiffrer = new();
            DB_Compte dbCompte = new(context);

            string hashCle = await dbCompte.GetHashCleAsync(idCompte);

            AESprotection aes = new(hashCle);

            foreach (string mail in _groupe.listeMail)
            {
                string mailDechiffrer = aes.Dechiffrer(mail);
                listeMailSecuDechiffrer.Add(Protection.XSS(mailDechiffrer));
            }

            string titreSecuDechiffrer = Protection.XSS(aes.Dechiffrer(_groupe.Titre));

            Groupe groupe = new()
            {
                Titre = aes.Chiffrer(titreSecuDechiffrer),
                IdCompteCreateur = idCompte
            };

            int idGroupe = await dbGroupe.AjouterAsync(groupe);


            if(listeMailSecuDechiffrer.Count > 0)
            {
                List<int> listeIdCompte = await dbGroupe.ListerIdCompteAsync(listeMailSecuDechiffrer);
                await dbGroupe.AjouterCompteGroupeAsync(listeIdCompte, idGroupe);
            }

            if(_groupe.listeIdMdp.Length > 0)
                await dbGroupe.AjouterMdpGroupeAsync(idGroupe, _groupe.listeIdMdp);

            return JsonConvert.SerializeObject(idGroupe);
        }

        [Authorize]
        [HttpDelete("supprimer/{idGroupe}")]
        public async Task<string> Supprimer([FromRoute] int idGroupe)
        {
            try
            {
                // recupere le token et vire le bearer
                string token = HttpContext.Request.Headers.Authorization;
                int idCompte = tokenNoConfig.GetIdCompte(token);

                await dbGroupe.SupprimerAsync(idGroupe, idCompte);

                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }

        [HttpPost("supprimerCompte")]
        public async Task<string> SupprimerMdp([FromBody] CompteGroupeImport _compteGroupe)
        {
            try
            {
                await dbGroupe.SupprimerCompteAsync(_compteGroupe.listeIdCompte, _compteGroupe.IdGroupe);

                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
