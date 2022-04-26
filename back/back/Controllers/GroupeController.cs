using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupeController : ControllerBase
    {
        public GroupeController(GestionMdpContext _context)
        {
            DB_Groupe.context = _context;
        }

        [HttpGet("lister/{idCompte}")]
        public string Lister([FromRoute] int idCompte)
        {
            var liste = DB_Groupe.ListerLesMiens(idCompte);

            return JsonConvert.SerializeObject(liste);
        }

        [HttpPost("ajouter")]
        public string Ajouter([FromBody] groupeImport _groupe)
        {
            Groupe groupe = new()
            {
                Titre = Protection.XSS(_groupe.Titre),
                IdCompteCreateur = _groupe.IdCreateur
            };

            int idGroupe = DB_Groupe.Ajouter(groupe);


            return JsonConvert.SerializeObject(idGroupe);
        }
    }
}
