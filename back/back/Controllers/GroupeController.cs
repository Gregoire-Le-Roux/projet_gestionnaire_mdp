using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupeController : ControllerBase
    {
        private readonly DB_Groupe dbGroupe;
        private readonly GestionMdpContext context;

        public GroupeController(GestionMdpContext _context, IConfiguration _config)
        {
            dbGroupe = new (_context, _config);
            context = _context;
        }

        [HttpGet("lister/{idCompte}")]
        public async Task<string> Lister([FromRoute] int idCompte)
        {
            var liste = await dbGroupe.ListerLesMiensAsync(idCompte);

            return JsonConvert.SerializeObject(liste);
        }

        [HttpPost("ajouter")]
        public async Task<string> Ajouter([FromBody] groupeImport _groupe)
        {
            List<string> listeMailSecuDechiffrer = new();
            DB_Compte dbCompte = new(context);

            string hashCle = await dbCompte.GetHashCleAsync(_groupe.IdCreateur);

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
                IdCompteCreateur = _groupe.IdCreateur
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

        [HttpDelete("supprimer/{idGroupe}")]
        public async Task<string> Supprimer([FromRoute] int idGroupe)
        {
            try
            {
                await dbGroupe.SupprimerAsync(idGroupe);

                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
