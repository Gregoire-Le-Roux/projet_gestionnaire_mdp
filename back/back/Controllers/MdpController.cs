using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MdpController : Controller
    {
        private DB_Mdp dbMdp;
        private GestionMdpContext mdpContext;

        public MdpController(GestionMdpContext _context)
        {
            mdpContext = _context;
            dbMdp = new(_context);
        }

        [HttpGet("listerMesMdp/{id}")]
        public string ListerMesMdp([FromRoute] int id)
        {
            MdpExport[] liste = dbMdp.ListerMesMdp(id);

            return JsonConvert.SerializeObject(liste);
        }

        [HttpGet("listerPartagerAvecMoi/{id}")]
        public string ListerPartagerAvecMoi([FromRoute] int id)
        {
            MdpExport[] liste = dbMdp.ListerMdpPartagerAvecMoi(id);

            return JsonConvert.SerializeObject(liste);
        }

        /// <summary>
        ///     Je c pas encore dans le body
        /// </summary>
        /// <param name="_info"></param>
        /// <returns></returns>
        [HttpPost("partager")]
        public string Partager([FromBody] dynamic _info)
        {
            return JsonConvert.SerializeObject("");
        }

        /// <summary>
        ///     Toutes les infos reçu doivent etre Chiffrés SAUF la date et l'id createur
        /// </summary>
        /// <param name="_mdp"></param>
        /// <response code="200">Id du mdp</response>
        [HttpPost("ajouter")]
        public string Ajouter(MdpImport _mdp)
        {
            DB_Compte.context = mdpContext;
            string hashCle = DB_Compte.GetHashCle(_mdp.IdCompteCreateur);

            AESprotection aes = new(hashCle);

            string date = Protection.XSS(aes.Dechiffrer(_mdp.DateExpiration));
            string titre = Protection.XSS(aes.Dechiffrer(_mdp.Titre));
            string login = Protection.XSS(aes.Dechiffrer(_mdp.Login));
            string mdpD = Protection.XSS(aes.Dechiffrer(_mdp.Mdp));
            string url = Protection.XSS(aes.Dechiffrer(_mdp.Url));
            string description = "";

            if(string.IsNullOrEmpty(_mdp.Description))
            {
                description = Protection.XSS(aes.Dechiffrer(_mdp.Description));
                description = aes.Chiffrer(_mdp.Description);
            }
                 
            MotDePasse mdp = new()
            {
                Titre = aes.Chiffrer(titre),
                Login = aes.Chiffrer(login),
                Mdp = aes.Chiffrer(mdpD),
                Url = aes.Chiffrer(url),
                DateExpiration = aes.Chiffrer(date),
                Description = description,
                IdCompteCreateur = _mdp.IdCompteCreateur
            };

            int id = dbMdp.Ajouter(mdp);

            return JsonConvert.SerializeObject(id);
        }
    }
}
