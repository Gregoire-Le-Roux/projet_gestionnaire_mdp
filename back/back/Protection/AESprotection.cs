using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace back.securite
{
    public class AESprotection
    {
        private AesCryptoServiceProvider aesCryptoService;

        /// <summary>
        ///     Initialise les parametres de AES
        /// </summary>
        /// <param name="_cleSecrete">La longeur doit etre de 32</param>
        /// <param name="_iv">La longeur doit etre de 16</param>
        public AESprotection(string _cleSecrete, string _iv)
        {
            aesCryptoService = new AesCryptoServiceProvider();

            aesCryptoService.BlockSize = 128;
            aesCryptoService.KeySize = 256;

            // longeur doit etre de 16
            aesCryptoService.IV = Encoding.UTF8.GetBytes(_iv);

            // longeur doit etre de 32
            aesCryptoService.Key = Encoding.UTF8.GetBytes(_cleSecrete);
            aesCryptoService.Mode = CipherMode.CBC;
            aesCryptoService.Padding = PaddingMode.PKCS7;
        }

        public string Chiffrer(string _text)
        {
            ICryptoTransform transform = aesCryptoService.CreateEncryptor();

            byte[] textByte = transform.TransformFinalBlock(Encoding.ASCII.GetBytes(_text), 0, _text.Length);

            string textBase64 = Convert.ToBase64String(textByte);

            // libere toutes les ressources utilisé
            aesCryptoService.Clear();

            return textBase64;
        }

        public string Dechiffrer(string _text)
        {
            ICryptoTransform transform = aesCryptoService.CreateDecryptor();

            byte[] textByte = Convert.FromBase64String(_text);

            byte[] textByteDechiffrer = transform.TransformFinalBlock(textByte, 0, textByte.Length);

            string textDechiffrer = Encoding.ASCII.GetString(textByteDechiffrer);

            // libere toutes les ressources utilisé
            aesCryptoService.Clear();

            return textDechiffrer;
        }

        public static string CreerCleChiffrement(string _nom, string _prenom, string _mail)
        {
            string info = _nom + _prenom + _mail;

            string cleChiffrement = BC.HashPassword(info);

            return cleChiffrement;
        }
    }
}
