namespace Barbu.Gameplay.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay;

    public interface IRound
    {
        Dictionary<string, int> PointMapping { get; }
        int PointsPerPile { get; }
        int TotalPoints { get; }
        string Name { get; }

        int CalculatePointsInPile(Pile pile);
        int CalculatePointsInAllPiles(List<Pile> piles);
        bool IsRoundOver(int round, Dictionary<string, int[]> points, int pilesplayed);
        bool IsRoundPositive();
        bool IsPointEarningCard(string cardName);
        int GetCardPointValue(string cardName);
    }
}
