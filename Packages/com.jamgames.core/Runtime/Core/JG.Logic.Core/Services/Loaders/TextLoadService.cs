using System.Text;
using Core.JG.Extensions;
using Core.JG.Logic.Core.Interfaces.Services.Loaders;
using IR.Presenters.Interfaces.Services.Resources;
using UnityEngine;

namespace Core.JG.Logic.Core.Services.Loaders
{
    public class TextLoadService : ITextLoadService
    {
        readonly IResourcesLoadService _resourcesLoadService;

        string _cache;
        StringBuilder _stringBuilder;
        public TextLoadService(IResourcesLoadService resourcesLoadService)
        {
            _resourcesLoadService = resourcesLoadService;
            _stringBuilder = new StringBuilder();
        }

        public string Load(string path, bool encrypted, bool optional = false)
        {
            var textAsset = _resourcesLoadService.Load<TextAsset>(path);
            if (textAsset == null)
            {
                if (!optional)
                {
                    // _logSystem.Error($"Wrong path: {path}");
                }

                return null;
            }
            
            var text = textAsset.text;
            if (encrypted)
                text = EncryptionExtension.EncryptDecrypt(_stringBuilder, text);

            return text;
        }
    }
}