using UnityEngine;
using ZW.Logic.Interfaces.Services.Environment;

namespace ZW.Logic.Services.Environment
{
    public class EnvironmentService : IEnvironmentService
    {
        public float DeltaTime => Time.deltaTime;
        public float Gravity => Mathf.Abs(Physics.gravity.y);
    }
}
