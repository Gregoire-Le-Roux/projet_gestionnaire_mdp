using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MdpController : Controller
    {
        private readonly Token tokenNoConfig;
        private readonly DB_Mdp dbMdp;
        private GestionMdpContext context;
        private IConfiguration config;

        public MdpController(GestionMdpContext _context, IConfiguration _config)
        {
            config = _config;
            context = _context;
            dbMdp = new(_context);
            tokenNoConfig = new();
        }

        [Authorize]
        [HttpGet("listerMesMdp")]
        public async Task<string> ListerMesMdp()
        {
            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            MdpExport[] liste = await dbMdp.ListerMesMdpAsync(idCompte);

            return JsonConvert.SerializeObject(liste);
        }

        [Authorize]
        [HttpGet("listerPartagerAvecMoi")]
        public async Task<string> ListerPartagerAvecMoi()
        {
            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            DB_Mdp dB_Mdp = new(context, config); 

            List<MdpExport> liste = await dB_Mdp.ListerMdpPartagerAvecMoi(idCompte);

            return JsonConvert.SerializeObject(liste);
        }

        [Authorize]
        [HttpPut("modifier")]
        public async Task<string> Modifier([FromBody] MdpImport _mdp)
        {
            // recupere le token
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            DB_Compte dbCompte = new(context);
            string hashCle = await dbCompte.GetHashCleAsync(idCompte);

            AESprotection aes = new(hashCle);

            _mdp.Titre = Protection.XSS(aes.Dechiffrer(_mdp.Titre));
            _mdp.Titre = aes.Chiffrer(_mdp.Titre);

            if(!string.IsNullOrEmpty(_mdp.Description))
            {
                _mdp.Description = Protection.XSS(aes.Dechiffrer(_mdp.Description));
                _mdp.Description = aes.Chiffrer(_mdp.Description);
            }

            if(!string.IsNullOrEmpty(_mdp.Login))
            {
                _mdp.Login = Protection.XSS(aes.Dechiffrer(_mdp.Login));
                _mdp.Login = aes.Chiffrer(_mdp.Login);
            }

            if(!string.IsNullOrEmpty(_mdp.Url))
            {
                _mdp.Url = Protection.XSS(aes.Dechiffrer(_mdp.Url));
                _mdp.Url = aes.Chiffrer(_mdp.Url);
            }

            if (!string.IsNullOrEmpty(_mdp.DateExpiration))
            {
                _mdp.DateExpiration = Protection.XSS(aes.Dechiffrer(_mdp.DateExpiration));
                _mdp.DateExpiration = aes.Chiffrer(_mdp.DateExpiration);
            }

            MotDePasse mdp = new()
            {
                Id = _mdp.Id,
                IdCompteCreateur = idCompte,
                DateExpiration = _mdp.DateExpiration,
                Mdp = _mdp.Mdp,
                Description = _mdp.Description,
                Titre = _mdp.Titre,
                Login = _mdp.Login,
                Url = _mdp.Url
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
        ///     Toutes les infos reçu doivent etre Chiffrés SAUF la date
        /// </summary>
        /// <param name="_mdp"></param>
        /// <response code="200">Id du mdp</response>
        [Authorize]
        [HttpPost("ajouter")]
        public async Task<string> Ajouter(MdpImport _mdp)
        {
            // recupere le token et vire le bearer
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            DB_Compte dbCompte = new(context);
            string hashCle = await dbCompte.GetHashCleAsync(idCompte);

            AESprotection aes = new(hashCle);

            string titre = Protection.XSS(aes.Dechiffrer(_mdp.Titre));

            if(!string.IsNullOrEmpty(_mdp.Description))
            {
                _mdp.Description = Protection.XSS(aes.Dechiffrer(_mdp.Description));
                _mdp.Description = aes.Chiffrer(_mdp.Description);
            }

            if(!string.IsNullOrEmpty(_mdp.DateExpiration))
            {
                _mdp.DateExpiration = Protection.XSS(aes.Dechiffrer(_mdp.DateExpiration));
                _mdp.DateExpiration = aes.Chiffrer(_mdp.DateExpiration);
            }

            if (!string.IsNullOrEmpty(_mdp.Login))
            {
                _mdp.Login = Protection.XSS(aes.Dechiffrer(_mdp.Login));
                _mdp.Login = aes.Chiffrer(_mdp.Login);
            }

            if (!string.IsNullOrEmpty(_mdp.Url))
            {
                _mdp.Url = Protection.XSS(aes.Dechiffrer(_mdp.Url));
                _mdp.Url = aes.Chiffrer(_mdp.Url);
            }

            MotDePasse mdp = new()
            {
                Titre = aes.Chiffrer(titre),
                Login = _mdp.Login,
                Mdp = _mdp.Mdp,
                Url = _mdp.Url,
                DateExpiration = _mdp.DateExpiration,
                Description = _mdp.Description,
                IdCompteCreateur = idCompte
            };

            int id = await dbMdp.AjouterAsync(mdp);

            return JsonConvert.SerializeObject(id);
        }

        [HttpDelete("supprimer/{idMdp}")]
        public async Task<string> Supprimer(int idMdp)
        {
            // recupere le token
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            bool estSupp = await dbMdp.SupprimerAsync(idMdp, idCompte, config.GetConnectionString("ionos"));

            return JsonConvert.SerializeObject(estSupp);
        }
    }
}
