﻿using Microsoft.AspNetCore.Mvc;

namespace back.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class GroupeController : ControllerBase
    {
        private readonly DB_Groupe dbGroupe;
        private readonly Token tokenNoConfig;
        private readonly GestionMdpContext context;
        private readonly IConfiguration config;

        public GroupeController(GestionMdpContext _context, IConfiguration _config)
        {
            tokenNoConfig = new();
            dbGroupe = new(_context, _config);
            context = _context;
            config = _config;
        }

        [HttpGet("lister")]
        public async Task<string> Lister()
        {
            // recupere le token
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            var liste = await dbGroupe.ListerLesMiensAsync(idCompte);

            return JsonConvert.SerializeObject(liste);
        }

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

        [HttpGet("existeCompte/{idGroupe}/{compteMail}")]
        public async Task<string> ExisteCompte([FromRoute] int idGroupe, string compteMail)
        {
            DB_Compte dbCompte = new(context);

            CompteExport compte = await dbCompte.GetCompteByMailAsync(Protection.XSS(compteMail));

            bool existe = await dbGroupe.ExisteCompteAsync(compte.Id, idGroupe);

            return JsonConvert.SerializeObject(existe);
        }

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
    
        [HttpPost("ajouterCompte")]
        public async Task<string> AjouterCompte([FromBody] groupeCompteImport _groupeCompte)
        {
            // recupere le token
            string token = HttpContext.Request.Headers.Authorization;
            int idCompte = tokenNoConfig.GetIdCompte(token);

            DB_Compte dB_Compte = new(context);
            CompteExport compte =  await dB_Compte.GetCompteByMailAsync(Protection.XSS(_groupeCompte.mail!));

            if (!await dbGroupe.EstAMoi(_groupeCompte.idGroupe, idCompte))
                return JsonConvert.SerializeObject(false);

            List<int> listeIdCompte = new();
            listeIdCompte.Add(compte.Id);

            await dbGroupe.AjouterCompteGroupeAsync(listeIdCompte, _groupeCompte.idGroupe);

            return JsonConvert.SerializeObject(compte);
        }

        [HttpPost("ajouterMdp")]
        public async Task<string> AjouterMdp([FromBody] groupeMdpImport _groupeMdp)
        {
            int[] listeIdMdp = _groupeMdp.ListeIdMdp;

            await dbGroupe.AjouterMdpGroupeAsync(_groupeMdp.IdGroupe, listeIdMdp);

            return JsonConvert.SerializeObject(true);
        }

        [HttpDelete("supprimer/{idGroupe}")]
        public async Task<string> Supprimer([FromRoute] int idGroupe)
        {
            try
            {
                // recupere le token
                string token = HttpContext.Request.Headers.Authorization;
                int idCompte = tokenNoConfig.GetIdCompte(token);

                if (!await dbGroupe.EstAMoi(idGroupe, idCompte))
                    return JsonConvert.SerializeObject("Vous n'êtes pas le propriétaire du groupe");

                await dbGroupe.SupprimerAsync(idGroupe, idCompte);

                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }

        [HttpPost("supprimerCompte")]
        public async Task<string> SupprimerCompte([FromBody] CompteGroupeImport _compteGroupe)
        {
            try
            {
                // recupere le token
                string token = HttpContext.Request.Headers.Authorization;
                int idCompte = tokenNoConfig.GetIdCompte(token);

                if (!await dbGroupe.EstAMoi(_compteGroupe.IdGroupe, idCompte))
                    return JsonConvert.SerializeObject("Vous n'êtes pas le propriétaire du groupe");

                await dbGroupe.SupprimerCompteAsync(_compteGroupe.listeIdCompte, _compteGroupe.IdGroupe);
                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }

        [HttpPost("supprimerMdp")]
        public async Task<string> SupprimerMdp([FromBody] MdpGroupeImport _mdpGroupe)
        {
            try
            {
                // recupere le token
                string token = HttpContext.Request.Headers.Authorization;
                int idCompte = tokenNoConfig.GetIdCompte(token);

                if (!await dbGroupe.EstAMoi(_mdpGroupe.IdGroupe, idCompte))
                    return JsonConvert.SerializeObject("Vous n'êtes pas le propriétaire du groupe");

                await dbGroupe.SupprimerMdpAsync(_mdpGroupe.ListeIdMdp, _mdpGroupe.IdGroupe);
                return JsonConvert.SerializeObject(true);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}
