using JG.Logic.Streams;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Streams.Animals
{
    public class AnimalMotionStream : StreamBase<AnimalMotionCommand>, IAnimalMotionStream
    {
    }
}
