using Microsoft.AspNetCore.Mvc;
using back.ImportModels;

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
            AESprotection p = new AESprotection("", "");

            return JsonConvert.SerializeObject(p.Chiffrer("salut"));
        }
    }
}
