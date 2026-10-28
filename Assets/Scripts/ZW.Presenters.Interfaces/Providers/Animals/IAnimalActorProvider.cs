using System.Collections.Generic;
using ZW.Views.Interfaces.Actors.Animals;

namespace ZW.Presenters.Interfaces.Providers.Animals
{
    public interface IAnimalActorProvider
    {
        IEnumerable<KeyValuePair<int, IAnimalActor>> All { get; }
        void Register(int uid, IAnimalActor actor);
        void Unregister(int uid);
        bool TryGet(int uid, out IAnimalActor actor);
        bool TryGetUid(IAnimalActor actor, out int uid);
    }
}
