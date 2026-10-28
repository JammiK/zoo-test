using ZW.Logic.Interfaces.Services.Environment;

namespace ZW.Logic.Tests.Fakes
{
    public class FakeEnvironmentService : IEnvironmentService
    {
        public float DeltaTime { get; set; }
        public float Gravity { get; set; } = 10f;
    }
}
