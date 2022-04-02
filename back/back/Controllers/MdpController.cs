using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MdpController : Controller
    {
        private GestionMdpContext mdpContext;

        public MdpController(GestionMdpContext _context)
        {
            mdpContext = _context;
            BD_Mdp.context = _context;
        }

        /// <summary>
        ///     Toutes les infos reçu doivent etre Chiffrés SAUF la date et l'id createur
        /// </summary>
        /// <param name="_mdp"></param>
        /// <response code="200">Id du mdp</response>
        [HttpPost("ajouter")]
        public string Ajouter(MdpImport _mdp)
        {
            MotDePasse mdp = new()
            {
                Titre = _mdp.Titre,
                Login = _mdp.Login,
                Mdp = _mdp.Mdp,
                Url = _mdp.Url,
                DateExpiration = DateTime.Parse(_mdp.DateExpiration),
                Description = _mdp.Description,
                IdCompteCreateur = _mdp.IdCompteCreateur
            };

            int id = BD_Mdp.Ajouter(mdp);

            return JsonConvert.SerializeObject(id);
        }
    }
}
