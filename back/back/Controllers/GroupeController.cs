using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupeController : ControllerBase
    {
        private readonly DB_Groupe dbGroupe;

        public GroupeController(GestionMdpContext _context, IConfiguration _config)
        {
            dbGroupe = new (_context, _config);
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
            List<string> listeMailSecu = new();

            foreach (string mail in _groupe.listeMail)
            {
                listeMailSecu.Add(Protection.XSS(mail));
            }

            Groupe groupe = new()
            {
                Titre = Protection.XSS(_groupe.Titre),
                IdCompteCreateur = _groupe.IdCreateur
            };

            int idGroupe = await dbGroupe.AjouterAsync(groupe);

            List<int> listeIdCompte = await dbGroupe.ListerIdCompteAsync(listeMailSecu);

            await dbGroupe.AjouterCompteGroupeAsync(listeIdCompte, idGroupe);
            await dbGroupe.AjouterMdpGroupeAsync(idGroupe, _groupe.IdMdp);

            return JsonConvert.SerializeObject(idGroupe);
        }
    }
}
