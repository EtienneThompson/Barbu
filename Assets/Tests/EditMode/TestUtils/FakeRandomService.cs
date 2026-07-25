namespace Barbu.Tests.EditMode.TestUtils
{
    using Barbu.Core;

    /// <summary>Deterministic IRandomService so tests aren't flaky.</summary>
    public class FakeRandomService : IRandomService
    {
        public int NextInt;
        public float NextFloat;
        public bool WasSeeded;

        public void SeedForNewGame() => this.WasSeeded = true;

        public int Range(int minInclusive, int maxExclusive) => this.NextInt;

        public float Range(float minInclusive, float maxInclusive) => this.NextFloat;

        public float Value() => this.NextFloat;
    }
}
