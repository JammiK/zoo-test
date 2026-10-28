using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JG.Data.Entities.Interfaces.Entities.Common;
using UnityEngine;

namespace JG.Data.Generators.Services
{
    public class GeneratorService
    {
        public static (CodeNamespace codeNamespace, 
            CodeTypeDeclaration targetClass, 
            List<PropertyInfo> properties) 
            GetNewClassConfig(Type entityInterface, string pureEntityPath, string className, bool ignoreRootNamespace = false)
        {
            var codeNamespace = new CodeNamespace(GetNamespace(pureEntityPath));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            if (entityInterface != null && !ignoreRootNamespace)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(entityInterface.Namespace));
            }

            var targetClass = new CodeTypeDeclaration(className);
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            
            var properties = new List<PropertyInfo>();

            if (entityInterface != null)
            {
                foreach (var parentInterface in entityInterface.GetInterfaces())
                {
                    foreach (var propertyInfo in parentInterface.GetProperties())
                    {
                        if (properties.Any(p => p.Name == propertyInfo.Name))
                        {
                            continue;
                        }
                        
                        properties.Add(propertyInfo);
                    }
                }
                
                foreach (var propertyInfo in entityInterface.GetProperties())
                {
                    if (properties.Any(p => p.Name == propertyInfo.Name))
                    {
                        continue;
                    }
                        
                    properties.Add(propertyInfo);
                }
            }

            return (codeNamespace, targetClass, properties);
        }

        public static (string path, string className) GetPath(Type entityInterfaces, string replacePattern, string replaceResult, string instancePatternName, string rootInstanceName)
        {
            var replaceResultSplited = replaceResult.Split(Path.DirectorySeparatorChar);
            var replaceResultSb = new StringBuilder();
            for (var i = 0; i < replaceResultSplited.Length - 1; i++)
            {
                replaceResultSb.Append(replaceResultSplited[i]).Append(Path.DirectorySeparatorChar);
                if (i == 0)
                {
                    replaceResultSb.Append("Generated").Append(Path.DirectorySeparatorChar);
                }
            }

            replaceResultSb.Append(replaceResultSplited.Last());

            replaceResult = replaceResultSb.ToString();

            var entityName = entityInterfaces.ToString().Split('.').Last();
            var projectDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (projectDirectory == null)
            {
                throw new Exception("Unable to get project directory");
            }
            var directoryInfos = projectDirectory.GetDirectories();
            var assetsDirectory = directoryInfos.FirstOrDefault(d => d.Name == "Assets");
            var scriptsDirectory = assetsDirectory?.GetDirectories().FirstOrDefault(d => d.Name.ToLower() == "scripts");
            var packagesDirectory = projectDirectory.GetDirectories().FirstOrDefault(d => d.Name.ToLower() == "packages");
            var directoriesForSearch = new List<string>();
            directoriesForSearch.Add(scriptsDirectory?.FullName);
            directoriesForSearch.Add(packagesDirectory?.FullName);
            directoriesForSearch.Add(assetsDirectory?.FullName);

            var filePath = string.Empty;
            foreach (var directory in directoriesForSearch)
            {
                if (string.IsNullOrEmpty(directory))
                {
                    continue;
                }
                
                var files = Directory.GetFiles(directory, $"{entityName}.cs", SearchOption.AllDirectories);
                switch (files.Length)
                {
                    case 0:
                        continue;
                    case 1:
                        filePath = files[0];
                        break;
                    default:
                    {
                        foreach (var file in files)
                        {
                            var classFileText = File.ReadAllText(file);
                            if (classFileText.Contains($"namespace {entityInterfaces.Namespace}"))
                            {
                                filePath = file;
                            }
                        }

                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception($"Unable to get right path for {entityInterfaces}");
            }
            
            var result = filePath.Replace(replacePattern, replaceResult);
            var parsedResult = result.Split(Path.DirectorySeparatorChar);
            var sb = new StringBuilder();
            
            for (var i = 0; i < parsedResult.Length - 1; i++)
            {
                sb.Append(parsedResult[i])
                    .Append(Path.DirectorySeparatorChar);
            }

            if (entityName[0] == 'I')
            {
                entityName = entityName.Substring(1);
            }

            if (entityName.Contains(rootInstanceName))
            {
                entityName = entityName.Remove(entityName.Length - rootInstanceName.Length);
            }

            var className = string.Format(instancePatternName, entityName);
            sb.Append(className);
            return (sb.ToString(), className);
        }

        public static string GetNamespace(string path)
        {
            var parts = path.Split(Path.DirectorySeparatorChar);
            var sb = new StringBuilder();
            var began = false;
            var startIndex = 0;
            for (var index = 0; index < parts.Length - 1; index++)
            {
                var part = parts[index];
                if (part.Contains("."))
                {
                    startIndex = index;
                }
            }

            for (var index = startIndex; index < parts.Length - 1; index++)
            {
                var part = parts[index];
                if (!began && !part.Contains("."))
                {
                    continue;
                }

                began = true;
                sb.Append(part);
                if (index != parts.Length - 2)
                {
                    sb.Append(".");
                }
            }
            
            return sb.ToString();
        }
    }
}