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
            DB_Compte.context = _context;
        }
    }
}
