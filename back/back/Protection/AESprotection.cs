using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BC = BCrypt.Net.BCrypt;

namespace back.securite
{
    public class AESprotection
    {
        private AesManaged aes;

        private byte[] cleSecrete;
        private byte[] ivSecrete;

        /// <summary>
        ///     Initialise les parametres de AES
        /// </summary>
        /// <param name="_cleSecrete">La longeur doit etre de 32</param>
        /// <param name="_iv">La longeur doit etre de 16</param>
        public AESprotection(string _cleSecrete, string _iv)
        {
            cleSecrete = Encoding.UTF8.GetBytes(_cleSecrete);
            ivSecrete = Encoding.UTF8.GetBytes(_iv);

            aes = new AesManaged();

            aes.Key = cleSecrete;
            aes.IV = ivSecrete;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
        }

        public string Chiffrer(string _text)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] textByte = Encoding.UTF8.GetBytes(_text);
            cs.Write(textByte, 0, textByte.Length);
            cs.FlushFinalBlock();

            byte[] chiffre = ms.ToArray();

            cs.Close();
            ms.Close();

            return Convert.ToBase64String(chiffre);
        }

        public string Dechiffrer(string _text)
        {
            string[] tabCaractereUTF16 = new string[] { "\n", "\t", "\u000b", "\u0007", "\x07" };

            aes.Padding = PaddingMode.Zeros;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] textByte = Convert.FromBase64String(_text);
            cs.Write(textByte, 0, textByte.Length);
            cs.FlushFinalBlock();

            byte[] dechiffrer = ms.ToArray();

            // ferme les flux
            cs.Close();
            ms.Close();

            string textClair = Encoding.UTF8.GetString(dechiffrer, 0, dechiffrer.Length);

            // netoie le text des caracteres UTF16 générer par le chiffrement
            foreach (string element in tabCaractereUTF16)
            {
                textClair = textClair.Replace(element, string.Empty);
            }

            return textClair;
        }

        public static string CreerCleChiffrement(string _nom, string _prenom, string _mail)
        {
            string info = _nom + _prenom + _mail;

            string cleChiffrement = BC.HashPassword(info);

            return cleChiffrement;
        }
    }
}
