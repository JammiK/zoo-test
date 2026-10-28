using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.JG.Extensions.Editor.Extensions;
using JG.Data.Generators.Extensions;
using Microsoft.CSharp;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace JG.Data.Generators.Buttons
{
    public class LocalizationButton
    {
        const string LocalizationCsvPath = "Assets/GameData/Localizations/Localizations.csv";
        const string KeyHeader = "Key";

        static readonly HashSet<string> _ignoreHeaders = new HashSet<string>
        {
            "Desc",
            "FR",
            "DE",
            "ES"
        };

        static bool IgnoreHeader(string h)
        {
            return h == KeyHeader || _ignoreHeaders.Contains(h);
        }
#if !DISABLE_CSV_HELPER
        [MenuItem("[JG]/Parse Localizations")]
        static void ParseLocalizations()
        {
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(LocalizationCsvPath).text;
            if (text == null)
            {
                throw new ArgumentException($"'{LocalizationCsvPath}' is wrong path for localization csv");
            }

            var result = CsvExtensions.GetParsedCsvWithKey(text, IgnoreHeader);

            foreach (var resultKey in result.Keys)
            {
                Dictionary<string, string> dictionary = result[resultKey];
                var json = JsonConvert.SerializeObject(dictionary);
                var path = $"Assets/Resources/Localizations/Localization_{resultKey}.json";
                CreateFolderForPath(path);
                var writer = new StreamWriter(path,
                    false);
                writer.Write(json);
                writer.Close();
            }

            AssetDatabase.Refresh();
            GenerateKeys();
            AssetDatabase.Refresh();
            Debug.Log("Locations Done");
        }
#endif

        static void GenerateKeys()
        {
            var targetUnit = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace("IR.Presenters.CodeGeneration.Localizations");
            samples.Imports.Add(new CodeNamespaceImport("System"));
            CodeTypeDeclaration targetClass = new CodeTypeDeclaration("LocalizationKeys");
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            samples.Types.Add(targetClass);
            targetUnit.Namespaces.Add(samples);

            var textAsset = Resources.Load<TextAsset>("Localizations/Localization_Russian");
            var localizations = JsonConvert.DeserializeObject<Dictionary<string, string>>(textAsset.text);
            foreach (var keyValuePair in localizations)
            {
                CodeMemberField keyField = new CodeMemberField();
                keyField.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                var strings = keyValuePair.Key.Split('.', '_');
                for (var i = 0; i < strings.Length; i++)
                {
                    strings[i] = strings[i].First().ToString().ToUpperInvariant() +
                                 string.Join("", strings[i].Skip(1)).ToLowerInvariant();
                }

                keyField.Name = string.Join("", strings);
                keyField.Type = new CodeTypeReference(typeof(System.String));
                keyField.InitExpression = new CodePrimitiveExpression(keyValuePair.Key);
                keyField.Comments.Add(new CodeCommentStatement($"{keyValuePair.Value}"));
                targetClass.Members.Add(keyField);
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Build the output file name.
            string sourceFileName;
            if (provider.FileExtension[0] == '.')
            {
                sourceFileName = "Assets/Scripts/IR.Logic.Interfaces/CodeGeneration/Localizations/LocalizationKeys" +
                                 provider.FileExtension;
            }
            else
            {
                sourceFileName = "Assets/Scripts/IR.Logic.Interfaces/CodeGeneration/Localizations/LocalizationKeys." +
                                 provider.FileExtension;
            }

            CreateFolderForPath(sourceFileName);
            CodeGenExtensions.WriteSourceFile(sourceFileName, provider, targetUnit);
        }
        
        static void CreateFolderForPath(string path)
        {
            var folder = path.Substring(0, path.Length - path.Split("/").Last().Length - 1);
            if (!AssetDatabase.IsValidFolder(folder))
            {
                folder.Split("/").Skip(1).Aggregate("Assets/", (current, folderName) =>
                {
                    current += folderName + "/";
                    if (!AssetDatabase.IsValidFolder(current))
                    {
                        AssetDatabase.CreateFolder(current.Substring(0, current.Length - folderName.Length - 2), folderName);
                    }
                    return current;
                });
            }
        }
    }
}
