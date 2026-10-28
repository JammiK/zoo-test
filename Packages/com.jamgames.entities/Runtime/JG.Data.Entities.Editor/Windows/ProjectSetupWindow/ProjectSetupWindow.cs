using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JG.Data.Entities.Editor.Windows.ProjectSetupWindow
{
    public class ProjectSetupWindow : EditorWindow
    {
        [MenuItem("[JG]/Project/Setup")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupWindow>("Project Setup");
        }

        public void OnEnable()
        {
            VisualElement root = rootVisualElement;

            var textField = new TextField("Input Field:");
            root.Add(textField);

            var button = new Button(() =>
            {
                if (!string.IsNullOrEmpty(textField.value))
                {
                    var projectLayerModels = new ProjectStructureModel(textField.value).GetStructure();
                    foreach (var projectLayerModel in projectLayerModels)
                    {
                        foreach (var asmDefModel in projectLayerModel.AsmDefsList)
                        {
                            Directory.CreateDirectory(asmDefModel.folder);
                            File.WriteAllText(asmDefModel.GetFullPath(), JsonUtility.ToJson(asmDefModel));    
                        }
                    }
                    
                    AssetDatabase.Refresh();
                }
            })
            {
                text = "Setup"
            };
        
            root.Add(button);
        }

    }
}
