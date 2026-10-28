using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.JG.Extensions;
using JG.Logic.Entities.Interfaces.Services.Saves;
using Newtonsoft.Json;
using UnityEngine;

namespace JG.Logic.Entities.Services.Saves
{
    public class GameSaveJsonService : IGameSaveJsonService
    {
        readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly StringBuilder _stringBuilder;
        readonly string _path;
        public GameSaveJsonService(IGameSavePlayerPrefsKeyService gameSavePlayerPrefsKeyService)
        {
            _stringBuilder = new StringBuilder();
            _path = Path.Combine(Application.persistentDataPath, gameSavePlayerPrefsKeyService.GetKey());

            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public bool CheckGameSaveExists()
        {
            return File.Exists(_path);
        }

        public  List<object> LoadGameSaveJson()
        {
            string json = IOExtensions.Get(_path);

            if (string.IsNullOrEmpty(json))
                return null;

            json = EncryptionExtension.EncryptDecrypt(_stringBuilder, json);
            var gameContextSave = JsonConvert.DeserializeObject<List<object>>(json, _jsonSerializerSettings);

            // Log.Info($"LoadGameSaveJson:{json}");

            return gameContextSave;
        }

        public void SaveGameContext(List<object> gameSaves)
        {
            var json = JsonConvert.SerializeObject(gameSaves, _jsonSerializerSettings);

            // Log.Info($"SaveGameContext:{json}");

            json = EncryptionExtension.EncryptDecrypt(_stringBuilder, json);
            if (IOExtensions.TrySaveText(_path, json) == false)
            {
                // Log.Error($"Failed to save game context:{json}");
            }
        }
    }
}
