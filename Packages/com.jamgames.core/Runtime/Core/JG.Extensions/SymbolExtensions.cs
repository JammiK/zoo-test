using System.Collections.Generic;
using System.Text;

namespace Core.JG.Extensions
{
    public class SymbolExtensions
    {
        public static List<string> ParseSymbols(string data)
        {
            var symbold = new List<string>();// StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < data.Length; i++)
                symbold.Add(data[i].ToString());
            return symbold;
        }

        public static bool CheckStringContainsSymbols(string text, IList<string> symbols)
        {
            foreach (var sym in symbols)
                if (text.Contains(sym))
                    return true;
            return false;
        }

        public static string FilterSymbols(string text, char replace, List<string> symbols)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                string s = text[i].ToString();
                if (symbols.Contains(s))
                    sb.Append(s);
                else
                    sb.Append(replace);
            }
            return sb.ToString();
        }

        public static string RemoveSymbols(string text, char replace, IList<string> symbols, bool userReplacement = true)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                string s = text[i].ToString();
                if (symbols.Contains(s))
                {
                    if (userReplacement)
                        sb.Append(replace);
                }
                else
                    sb.Append(s);
            }
            return sb.ToString();
        }
    }
}
