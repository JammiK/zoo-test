using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.Movements;

namespace ZW.Logic.Tests.Fakes
{
    public class FakeRandomDirectionService : IRandomDirectionService
    {
        public PlanarVector Direction { get; set; } = new PlanarVector(1f, 0f);

        public PlanarVector Next() => Direction;
    }
}
