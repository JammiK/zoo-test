using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JG.Data.Entities.Editor.Windows.ProjectSetupWindow
{
    public class AsmDefModel
    {
        public string name;
        public List<string> references = new();
        public List<string> includePlatforms = new();
        
        [NonSerialized]
        public string folder;

        public AsmDefModel(string name, string folder, bool isEditor)
        {
            this.name = name;
            this.folder = folder;
            if (isEditor)
            {
                includePlatforms.Clear();
                includePlatforms.Add("Editor");
            }
        }

        public void AddReference(AsmDefModel asmDefModel)
        {
            if (asmDefModel == null)
            {
                return;
            }
            
            if (!references.Contains(asmDefModel.name))
            {
                references.Add(asmDefModel.name);
            }
        }
        
        public string GetFullPath()
        {
            return GetFullPath(folder, name);
        }
        
        public static string GetFullPath(string asmFolder, string asmName)
        {
            return $"{asmFolder}{Path.DirectorySeparatorChar}{asmName}.asmdef";
        }

        public static AsmDefModel TryLoad(string asmFolder, string asmName)
        {
            return TryLoad(GetFullPath(asmFolder, asmName));
        }
        
        public static AsmDefModel TryLoad(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<AsmDefModel>(json);
        }
    }
}