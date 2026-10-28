using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JG.Data.Entities.Editor.Windows.ProjectSetupWindow
{
    public class ProjectLayerModel
    {
        public AsmDefModel AsmDef;
        public AsmDefModel InterfacesAsmDef;
        public AsmDefModel EditorAsmDef;
        public AsmDefModel ModelsAsmDef;
        public List<AsmDefModel> AsmDefsList = new();

        public ProjectLayerModel(string layerName, bool withInterfaces, bool withEditor = false, bool withModels = false)
        {
            AsmDef = CreateAsmDef(layerName, GetFolder(layerName));
            if (withModels)
            {
                ModelsAsmDef = CreateAsmDef($"{layerName}.Models", GetFolder(layerName, "Models"));
                AsmDef.AddReference(ModelsAsmDef);
            }
            
            if (withInterfaces)
            {
                InterfacesAsmDef = CreateAsmDef($"{layerName}.Interfaces", GetFolder(layerName, "Interfaces"));
                AsmDef.AddReference(InterfacesAsmDef);
                if (withModels)
                {
                    InterfacesAsmDef.AddReference(ModelsAsmDef);
                }
            }
            
            if (withEditor)
            {
                EditorAsmDef = CreateAsmDef($"{layerName}.Editor", GetFolder(layerName, "Editor"), true);
                EditorAsmDef.AddReference(AsmDef);
                if (withInterfaces)
                {
                    EditorAsmDef.AddReference(InterfacesAsmDef);
                }
                
                if (withModels)
                {
                    EditorAsmDef.AddReference(ModelsAsmDef);
                }
            }
        }

        public void AddLayerDependency(ProjectLayerModel projectLayerModel)
        {
            InterfacesAsmDef?.AddReference(projectLayerModel.InterfacesAsmDef);
            InterfacesAsmDef?.AddReference(projectLayerModel.ModelsAsmDef);

            AsmDef?.AddReference(projectLayerModel.InterfacesAsmDef);
            AsmDef?.AddReference(projectLayerModel.ModelsAsmDef);

            EditorAsmDef?.AddReference(projectLayerModel.EditorAsmDef);
            EditorAsmDef?.AddReference(projectLayerModel.InterfacesAsmDef);
            EditorAsmDef?.AddReference(projectLayerModel.ModelsAsmDef);
        }

        AsmDefModel CreateAsmDef(string name, string folder, bool isEditor = false)
        {
            var asmDef = AsmDefModel.TryLoad(name, folder) ?? new AsmDefModel(name, folder, isEditor);
            AsmDefsList.Add(asmDef);
            return asmDef;
        }

        string GetFolder(string layerName, string postfix = "")
        {
            if (string.IsNullOrEmpty(postfix))
            {
                return
                    $"{Application.dataPath}{Path.DirectorySeparatorChar}Scripts{Path.DirectorySeparatorChar}{layerName}";
            }
            
            return
                $"{Application.dataPath}{Path.DirectorySeparatorChar}Scripts{Path.DirectorySeparatorChar}{layerName}.{postfix}";
        }

        public void AddDependency(ProjectLayerModel dependency)
        {
            AsmDef.references.Add(dependency.AsmDef.name);
        }
        
        public void AddBaseDependency(string dependency)
        {
            AsmDef.references.Add(dependency);
        }
    }
}