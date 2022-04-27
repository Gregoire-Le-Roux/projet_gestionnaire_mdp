using System.Security.Cryptography;
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
        /// <param name="_cleSecrete">Superieur à 32 de longeur</param>
        public AESprotection(string _cleSecrete)
        {
            cleSecrete = Encoding.UTF8.GetBytes(_cleSecrete.Substring(0, 32));
            ivSecrete = Encoding.UTF8.GetBytes(_cleSecrete.Substring(0, 16));

            aes = new AesManaged
            {
                Key = cleSecrete,
                IV = ivSecrete,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
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
            string[] tabCaractereUTF16Asupprimer = new string[] 
            { "\n", "\t", "\f", "\b", "\u000b", "\u0007", "\x07", "\u0006", "\u000e", "\u001f", "\x1F" };

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

            // netoie le text des caracteres UTF16 généré par le chiffrement
            foreach (string element in tabCaractereUTF16Asupprimer)
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
