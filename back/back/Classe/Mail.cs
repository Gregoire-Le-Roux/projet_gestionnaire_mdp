using MimeKit;
using MailKit.Net.Smtp;

namespace back.Classe
{
    public class Mail
    {
        private string MDP { get; init; }
        private string MAIL { get; init; }
        private string  URL_BASE { get; set; }
        private string SMTP_SERVEUR { get; init; }

        private const int SMTP_PORT = 587;

        private string Destinataire { get; init; }
        private string Message { get; init; }
        private string Sujet { get; init; }
        private string Token { get; init; }

        public string Nom { get; init; } = "";
        public string Prenom { get; init; } = "";

        private Mail(IConfiguration _config)
        {
            if(_config.GetValue<bool>("estModeProd"))
            {
                MDP = _config.GetValue<string>("mailProd:mdp");
                MAIL = _config.GetValue<string>("mailProd:login");
                URL_BASE = _config.GetValue<string>("mailProd:baseUrl");
                SMTP_SERVEUR = _config.GetValue<string>("mailProd:smtp");
            }
            else
            {
                MDP = _config.GetValue<string>("mail:mdp");
                MAIL = _config.GetValue<string>("mail:login");
                URL_BASE = _config.GetValue<string>("mail:baseUrl");
                SMTP_SERVEUR = _config.GetValue<string>("mail:smtp");
            }
        }

        /// <summary>
        ///     Constructeur pour envoir mail personnalisé
        /// </summary>
        public Mail(string _destinataire, string _message, string _sujet, IConfiguration _config): this(_config)
        {
            Destinataire = _destinataire;
            Message = _message;
            Sujet = _sujet;
        }

        /// <summary>
        ///     Constructeur pour envoie mail de confirmation d'inscription
        /// </summary>
        public Mail(string _destinataire, string _token, IConfiguration _config): this(_config)
        {
            Destinataire = _destinataire;
            Token = _token;
        }

        public async Task EnvoyerAsync()
        {
            string text = "";
            string sujet = "";

            BodyBuilder bodyBuilder = new();

            if (!string.IsNullOrEmpty(Token))
            {
                text = $"Bonjour {Prenom} {Nom} \n\n" +
                    $"Afin de confirmer votre demande d'inscription cliquer sur le lien ci-dessous \n " +
                    $"<a href='{URL_BASE}compteValider/{Token}'>Confirmer ma demande d'inscription</a> \n" +
                    $"Ce lien est valable 30 minutes \n\n" +
                    $"A bientôt sur PasseBase !";

                sujet = "Confirmation de votre compte sur passeBase";
            }
            else
            {
                text = Message;
                sujet = Sujet;
            }

            bodyBuilder.HtmlBody = text;

            MimeMessage mimeMsg = new()
            {
                Subject = sujet,
                Body = bodyBuilder.ToMessageBody()
            };

            mimeMsg.From.Add(new MailboxAddress("Expediteur", MAIL));
            mimeMsg.To.Add(new MailboxAddress("Destinataire", Destinataire));

            SmtpClient smtp = new()
            {
                CheckCertificateRevocation = false
            };

            await smtp.ConnectAsync(SMTP_SERVEUR, SMTP_PORT);

            await smtp.AuthenticateAsync(MAIL, MDP);
            await smtp.SendAsync(mimeMsg);
            await smtp.DisconnectAsync(true);
        }
    }
}
