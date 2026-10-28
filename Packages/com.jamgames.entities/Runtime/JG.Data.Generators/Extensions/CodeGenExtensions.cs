using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace JG.Data.Generators.Extensions
{
    public class CodeGenExtensions
    {
        public static void WriteSourceFile(string sourceFilePath, CSharpCodeProvider provider, CodeCompileUnit targetUnit)
        {
            var directoryName = Path.GetDirectoryName(sourceFilePath);
            if (!Directory.Exists(directoryName))
            {
                var parts = directoryName.Split('/');
                AssetDatabaseUtility.CreateDirectoriesInPath(directoryName);
                var sb = new StringBuilder();
                var began = false;
                foreach (var part in parts)
                {
                    if (!began && !part.Contains("Assets"))
                    {
                        continue;
                    }

                    began = true;
                    sb.Append(part).Append("/");
                }
                
                // AssetDatabaseUtility.CreateDirectoriesInPath(sb.ToString());
                // AssetDatabase.Refresh();
            }
            
            // Create a TextWriter to a StreamWriter to the output file.
            using (StreamWriter sw = new StreamWriter(sourceFilePath, false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
                var codeGeneratorOptions = new CodeGeneratorOptions();
                codeGeneratorOptions.BracingStyle = "C";
                // Generate source code using the code provider.
                provider.GenerateCodeFromCompileUnit(targetUnit, tw,
                    codeGeneratorOptions);

                // Close the output file.
                tw.Close();
            }
        }
    }
}