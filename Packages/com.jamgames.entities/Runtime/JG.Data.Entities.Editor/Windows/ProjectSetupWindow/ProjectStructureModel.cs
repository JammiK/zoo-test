using System.Collections.Generic;

namespace JG.Data.Entities.Editor.Windows.ProjectSetupWindow
{
    public class ProjectStructureModel
    {
        private readonly string _projectPrefix;
        
        public List<ProjectLayerModel> Layers { get; } = new();

        public ProjectStructureModel(string projectPrefix)
        {
            _projectPrefix = projectPrefix;
        }

        public IEnumerable<ProjectLayerModel> GetStructure()
        {
            Layers.Clear();

            var dataLayer = AddLayer("Data", true, true, true);
            var logicLayer = AddLayer("Logic", true, false, true);
            var presentersLayer = AddLayer("Presenters", true, false, false);
            var viewsLayer = AddLayer("Views", true, false, true);
            
            logicLayer.AddLayerDependency(dataLayer);
            presentersLayer.AddLayerDependency(dataLayer);
            presentersLayer.AddLayerDependency(logicLayer);
            presentersLayer.AddLayerDependency(viewsLayer);
            viewsLayer.AddLayerDependency(dataLayer);

            return Layers;
        }

        ProjectLayerModel AddLayer(string layerName, bool withInterfaces, bool withEditor = false, bool withModels = false)
        {
            var targetName = $"{_projectPrefix}.{layerName}";
            var projectLayerModel = new ProjectLayerModel(targetName, withInterfaces, withEditor, withModels);
            Layers.Add(projectLayerModel);
            return projectLayerModel;
        }
    }
}