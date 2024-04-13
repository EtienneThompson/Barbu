namespace Barbu.Interfaces.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;

    public interface IRound
    {
        Dictionary<string, int> PointMapping { get; }
        int PointsPerPile { get; }
        int TotalPoints { get; }
        string Name { get; }
        void GoNext();
        void SetNextState(Round next);
        int CalculatePointsInPile(Pile pile);
        int CalculatePointsInAllPiles(List<Pile> piles);
        bool IsRoundOver(int round, Dictionary<string, int[]> points, int pilesplayed);

        bool IsRoundPositive();
        bool IsPointEarningCard(string cardName);
        int GetCardPointValue(string cardName);
    }
}
