using MailKit;
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
        private string token { get; init; } = "";

        public Mail(string _destinataire)
        {
            Destinataire = _destinataire;
        }

        public Mail(string _destinataire, string _token) : this(_destinataire)
        {
            token = _token;
        }

        public async Task EnvoyerAsync(bool _estMailConfirmationInscription = false)
        {
            string text = "";
            string sujet = "";

            BodyBuilder bodyBuilder = new();

            if(_estMailConfirmationInscription)
            {
                text = $"click: <a href='http://localhost:4200/#/compteValider/{token}'>click ici</a>";
                sujet = "Confirmation de votre compte";
            }   
            else
            {
                text = "salut !";
                sujet = "sujet mail";
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
