using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MdpController : Controller
    {
        private DB_Mdp dbMdp;
        private GestionMdpContext context;

        public MdpController(GestionMdpContext _context)
        {
            context = _context;
            dbMdp = new(_context);
        }

        [HttpGet("listerMesMdp/{id}")]
        public async Task<string> ListerMesMdp([FromRoute] int id)
        {
            MdpExport[] liste = await dbMdp.ListerMesMdpAsync(id);

            return JsonConvert.SerializeObject(liste);
        }

        [HttpGet("listerPartagerAvecMoi/{id}")]
        public string ListerPartagerAvecMoi([FromRoute] int id)
        {
            MdpExport[] liste = dbMdp.ListerMdpPartagerAvecMoi(id);

            return JsonConvert.SerializeObject(liste);
        }

        [HttpPut("modifier")]
        public async Task<string> Modifier([FromBody] MdpImport _mdp)
        {
            DB_Compte dbCompte = new(context);
            string hashCle = await dbCompte.GetHashCleAsync(_mdp.IdCompteCreateur);

            AESprotection aes = new(hashCle);
            _mdp.Description = Protection.XSS(aes.Dechiffrer(_mdp.Description));
            _mdp.Titre = Protection.XSS(aes.Dechiffrer(_mdp.Titre));
            _mdp.Login =  Protection.XSS(aes.Dechiffrer(_mdp.Login));
            _mdp.Url =  Protection.XSS(aes.Dechiffrer(_mdp.Url));

            MotDePasse mdp = new()
            {
                Id = _mdp.Id,
                IdCompteCreateur = _mdp.IdCompteCreateur,
                DateExpiration = _mdp.DateExpiration,
                Mdp = _mdp.Mdp,
                Description = aes.Chiffrer(_mdp.Description),
                Titre = aes.Chiffrer(_mdp.Titre),
                Login = aes.Chiffrer(_mdp.Login),
                Url = aes.Chiffrer(_mdp.Url)
            };

            await dbMdp.ModifierAsync(mdp);

            return JsonConvert.SerializeObject(true);
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
        public async Task<string> Ajouter(MdpImport _mdp)
        {
            DB_Compte dbCompte = new(context);
            string hashCle = await dbCompte.GetHashCleAsync(_mdp.IdCompteCreateur);

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

            int id = await dbMdp.AjouterAsync(mdp);

            return JsonConvert.SerializeObject(id);
        }
    }
}
