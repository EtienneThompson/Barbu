using System.Collections;
using System.Collections.Generic;

public interface IRound
{
    Dictionary<string, int> PointMapping { get; }
    int PointsPerPile { get; }
    int TotalPoints { get; }
    string Name { get; }
    void GoNext();
    void SetNextState(Round next);
    int CalculatePointsInPile(Card[] pile);
    int CalculatePointsInAllPiles(List<Card[]> piles);
    bool IsRoundOver(int round, Dictionary<string, int[]> points);
}
