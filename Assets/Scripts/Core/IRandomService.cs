namespace Barbu.Core
{
    public interface IRandomService
    {
        void SeedForNewGame();

        int Range(int minInclusive, int maxExclusive);

        float Range(float minInclusive, float maxInclusive);

        float Value();
    }
}
