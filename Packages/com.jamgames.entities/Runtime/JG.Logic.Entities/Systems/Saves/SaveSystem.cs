using System;
using System.Collections.Generic;
using JG.Data.Entities.Interfaces.Providers.Contexts;
using JG.Data.Entities.Interfaces.Systems.Saves;
using JG.Logic.Entities.Interfaces.Services.Saves;
using UniRx;
using UnityEngine;

namespace JG.Logic.Entities.Systems.Saves
{
    public class SaveSystem : ISaveSystem
    {
        readonly IGameSaveJsonService _gameSaveJsonService;
        readonly IContextProvider _contextProvider;

        bool locked_;
        IDisposable _saveRequestStream;

        public SaveSystem(IGameSaveJsonService gameSaveJsonService,
            IContextProvider contextProvider)
        {
            _gameSaveJsonService = gameSaveJsonService;
            _contextProvider = contextProvider;
        }

        public void RequestSave()
        {
            if (_saveRequestStream != null)
            {
                return;
            }
            
            _saveRequestStream = Observable.Range(0, 1)
                .DelayFrame(1)
                .Subscribe(_ => Save());
        }

        public void SetLocked(bool locked)
        {
            locked_ = locked;
        }

        void Save()
        {
            if (locked_)
            {
                _saveRequestStream?.Dispose();
                _saveRequestStream = null;  
                return;
            }
            
            if (_contextProvider == null)
            {
                Debug.LogError("Unable to save contexts. ContextProvider is not initialized.");
                return;
            }
            
            var saves = new List<object>(_contextProvider.Contexts.Count);
            foreach (var kvp in _contextProvider.Contexts)
            {
                if (kvp.Value.IgnoreSave)
                {
                    continue;
                }
                var save = kvp.Value.CreateSave(kvp.Key);
                saves.Add(save);
            }

            _gameSaveJsonService.SaveGameContext(saves);
            _saveRequestStream?.Dispose();
            _saveRequestStream = null;
        }
    }
}
