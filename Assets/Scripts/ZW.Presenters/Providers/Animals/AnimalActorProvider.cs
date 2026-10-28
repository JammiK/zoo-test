using System.Collections.Generic;
using ZW.Presenters.Interfaces.Providers.Animals;
using ZW.Views.Interfaces.Actors.Animals;

namespace ZW.Presenters.Providers.Animals
{
    public class AnimalActorProvider : IAnimalActorProvider
    {
        private readonly Dictionary<int, IAnimalActor> _actors = new();
        private readonly Dictionary<IAnimalActor, int> _uids = new();

        public IEnumerable<KeyValuePair<int, IAnimalActor>> All => _actors;

        public void Register(int uid, IAnimalActor actor)
        {
            _actors[uid] = actor;
            _uids[actor] = uid;
        }

        public void Unregister(int uid)
        {
            if (_actors.Remove(uid, out var actor))
            {
                _uids.Remove(actor);
            }
        }

        public bool TryGet(int uid, out IAnimalActor actor)
        {
            return _actors.TryGetValue(uid, out actor);
        }

        public bool TryGetUid(IAnimalActor actor, out int uid)
        {
            return _uids.TryGetValue(actor, out uid);
        }
    }
}
