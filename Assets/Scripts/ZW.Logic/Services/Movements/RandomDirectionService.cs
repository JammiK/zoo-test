using System;
using Core.JG.Data.Interfaces.Services;
using ZW.Data.Models.Types;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Movements;

namespace ZW.Logic.Services.Movements
{
    public class RandomDirectionService : IRandomDirectionService
    {
        private readonly IRandomService _randomService;

        public RandomDirectionService(IRandomService randomService)
        {
            _randomService = randomService;
        }

        public PlanarVector Next()
        {
            return PlanarVectorExtensions.FromAngle(_randomService.Range(0f, 2f * (float)Math.PI));
        }
    }
}
