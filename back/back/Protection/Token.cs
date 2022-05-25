using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace back.securite;

public class Token
{
    private IConfiguration config;
    private const string CLE_SECRETE =  "Y_cyZr5Gt#KJ9E%Ci:-Bu)XS?943bu_!Ucg:9.pJTg/8[23}5=WY4:Z?!G.#&;*;Z7KbY49<ySvy6s7:_" +
                                        "hbNpYfhY6E>DG6(28p2$%/7N:D29-as4?esV9>Cy4t@S|}*st]LGa9H@we^_{ee6fU*Wvcq85438-93!5" +
                                        "/63k4y9}ZW3E%U5zic)y8]4dtV952T{?%?~RY/.B@wS9Ns3M5NtYw8qe3rC;?Q3g2L9UE8yykpzFYQ3G7" +
                                        "J28Ts6=LR)up9sed{r2p~^8A4kBL~mQeTG)a65GpZeN}*6C3|9P3$>=Chg]Hsj42M?5h8?64F267|}4ER" +
                                        "h6Q,/7(~9]54~zKS4Mw392G&8>K88kUtykp!5EHpS85}T4pCM{QA8tp[7;KW9y+gm2mMP4yR6Yh[QP6-a4" +
                                        "j6t9w6h|?;iG;9|(](?9ytw6Qu4uBd:y:@58kXk]3MT;.R_n%9ecVe!7|mQLACV;%5vT9R&4.h|t_4LX8}" +
                                        "3#x^32M|dn}LSS^BsiuGA53z";

    public Token() { }

    public Token(IConfiguration _config)
    {
        config = _config;
    }

    public string Generer(int _idCompte)
    {
        var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CLE_SECRETE));

        var cleSigner = new SigningCredentials(cle, SecurityAlgorithms.HmacSha256);

        // ajout des infos divers dans le token
        Claim[] claim = new[]
        {
            new Claim("idCompte", _idCompte.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config.GetValue<string>("token:issuer"),
            audience: config.GetValue<string>("token:audience"),
            claims: claim,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: cleSigner
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string Generer(CompteImport _compte)
    {
        SigningCredentials cleSigner = GenererCleSigner();

        // ajout des infos divers dans le token
        Claim[] claim = new[]
        {
            new Claim("Prenom", _compte.Prenom),
            new Claim("Nom", _compte.Nom),
            new Claim("Mail", _compte.Mail),
            new Claim("Mdp", _compte.Mdp)
        };

        var token = new JwtSecurityToken(
            issuer: config.GetValue<string>("token:issuer"),
            audience: config.GetValue<string>("token:audience"),
            claims: claim,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: cleSigner
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int GetIdCompte(string _token)
    {
        _token = _token.Replace("Bearer ", string.Empty);

        // decode le token
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(_token.Trim());

        // recupere se qu'on veut dans le token
        string idCompteString = jsonToken.Claims.First(c => c.Type == "idCompte").Value;

        return int.Parse(idCompteString);
    }

    public SigningCredentials GenererCleSigner()
    {
        var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CLE_SECRETE));

        SigningCredentials cleSigner = new SigningCredentials(cle, SecurityAlgorithms.HmacSha256);

        return cleSigner;
    }
}

