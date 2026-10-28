using System.Text;

namespace Core.JG.Extensions
{
    public static class EncryptionExtension
    {
         const string EncryptKey = "pWXTBrjwniVXDPKuygp2";

         public static string EncryptDecrypt(StringBuilder sb, string input)
        {
            return EncryptDecrypt(sb, input, EncryptKey);
        }

         public static string EncryptDecrypt(StringBuilder sb, string input, string encrypt)
         {
             sb.Clear();
             int encryptLength = encrypt.Length;
             for (int i = 0; i < input.Length; i++)
             {
                 char c = (char)(input[i] ^ encrypt[i % encryptLength]);
                 sb.Append(c);
             }

             return sb.ToString();
         }
    }
}
