using System;
using JG.Logic.Entities.Interfaces.Handlers.Syncs;
using JG.Logic.Entities.Interfaces.Streams;
using JG.Logic.Entities.Interfaces.Systems.Syncs;
using UniRx;

namespace JG.Logic.Entities.Systems.Syncs
{
    public class SyncHandleSystem<T> : ISyncHandleSystem where T : Enum
    {
        public SyncHandleSystem(ISyncStream<T> syncStream, 
            ISyncHandler<T>[] syncHandlers)
        {
            if (syncStream == null)
            {
                return;
            }
            
            syncStream.Subscribe(syncFlag =>
            {
                for (var index = 0; index < syncHandlers.Length; index++)
                {
                    var syncHandler = syncHandlers[index];
                    if (syncHandler.SyncFlag.HasFlag(syncFlag) 
                        || (Convert.ToInt32(syncFlag) & Convert.ToInt32(syncHandler.SyncFlag)) > 0)
                    {
                        syncHandler.HandleSync(syncFlag);
                    }
                }
            });
        }
    }
}