using MimeKit;
using MailKit.Net.Smtp;

namespace back.Classe
{
    public class Mail
    {
        private string MDP = "R!@JAX46Zl60d!2tqA";
        private const string MAIL = "passebase@gmail.com";

        private const string SMTP_SERVEUR = "smtp.gmail.com";
        private const int SMTP_PORT = 465;

        private string Destinataire { get; init; }
        private string Message { get; init; }
        private string Sujet { get; init; }

        private string Token { get; init; }

        public string Nom { get; init; } = "";
        public string Prenom { get; init; } = "";

        /// <summary>
        ///     Constructeur pour envoir mail personnalisé
        /// </summary>
        public Mail(string _destinataire, string _message, string _sujet)
        {
            Destinataire = _destinataire;
            Message = _message;
            Sujet = _sujet;
        }

        /// <summary>
        ///     Constructeur pour envoie mail de confirmation d'inscription
        /// </summary>
        public Mail(string _destinataire, string _token)
        {
            Destinataire = _destinataire;
            Token = _token;
        }

        public async Task EnvoyerAsync()
        {
            string text = "";
            string sujet = "";

            BodyBuilder bodyBuilder = new();

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine(Token);

            if(!string.IsNullOrEmpty(Token))
            {
                text = $"Bonjour {Prenom} {Nom} \n\n" +
                    $"Afin de confirmer votre demande d'inscription cliquer sur le lien ci-dessous \n " +
                    $"<a href='http://localhost:4200/#/compteValider/{Token}'>Confirmer ma demande d'inscription</a> \n" +
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
