using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Operations
{
    public static class NameOperation
    {
        public static string CharacterRegulatory(string name)
        {
            string cleanName = name.Replace("Ö", "o")
                       .Replace("ö", "o")
                       .Replace("Ü", "u")
                       .Replace("ü", "u")
                       .Replace("Ğ", "g")
                       .Replace("ğ", "g")
                       .Replace("ç", "c")
                       .Replace("Ç", "c")
                       .Replace("Ş", "s")
                       .Replace("ş", "s")
                       .Replace("İ", "i")
                       .Replace("ı", "i")
                       .Replace("!", "")
                       .Replace("?", "")
                       .Replace("+", "")
                       .Replace("/", "")
                       .Replace("*", "")
                       .Replace("=", "")
                       .Replace("&", "")
                       .Replace("%", "")
                       .Replace("(", "")
                       .Replace(")", "")
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace("@", "")
                       .Replace("|", "")
                       .Replace("$", "")
                       .Replace("§", "")
                       .Replace("^", "")
                       .Replace("'", "")
                       .Replace("\"", "")
                       .Replace("_", "-")
                       .Replace(" ", "-");
                       






            return cleanName;
        }
    }
}
