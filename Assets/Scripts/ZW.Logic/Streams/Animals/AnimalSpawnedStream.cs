using JG.Logic.Streams;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Interfaces.Streams.Animals;

namespace ZW.Logic.Streams.Animals
{
    public class AnimalSpawnedStream : StreamBase<IAnimalModel>, IAnimalSpawnedStream
    {
    }
}
