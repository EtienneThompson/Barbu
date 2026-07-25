namespace Barbu.Core
{
    using System;

    public class RandomService : IRandomService
    {
        public void SeedForNewGame()
        {
            var seed = Environment.TickCount ^ Guid.NewGuid().GetHashCode();
            UnityEngine.Random.InitState(seed);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxInclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxInclusive);
        }

        public float Value()
        {
            return UnityEngine.Random.value;
        }
    }
}
