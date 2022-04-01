using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

namespace back.securite
{
    public static class Protection
    {
        public static string XSS(string _text)
        {
            if (string.IsNullOrEmpty(_text))
                return "";

            Regex regHtml = new Regex("<[^>]*>");
            return regHtml.Replace(_text, "");
        }

        public static string Image(IFormFile _fichier, int _poidsMax, string[] _listeExtension)
        {
            var p = new FileExtensionContentTypeProvider();
            p.TryGetContentType(_fichier.FileName, out string typeMime);

            if (_fichier.Length <= _poidsMax)
            {
                // verifie le type mime de l'image
                if (typeMime == "image/png")
                {
                    // verifie l'extention
                    if (_listeExtension.Contains(Path.GetExtension(_fichier.FileName)))
                    {
                        return "ok";
                    }
                    else
                    {
                        return "extension incorrect";
                    }
                }
                else
                {
                    return "Le format de l'image doit etre PNG";
                }
            }
            else
            {
                return "Le poids du fichier ne peut pas etre plus grand que: " + _poidsMax;
            }
        }
    }
}
