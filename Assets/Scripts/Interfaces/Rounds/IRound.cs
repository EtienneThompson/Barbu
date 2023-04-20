using System.Collections;
using System.Collections.Generic;

public interface IRound
{
    void GoNext();
    void SetNextState(Round next);
    int CalculatePointsInPile(Card[] pile);
    int CalculatePointsInAllPiles(List<Card[]> piles);
    bool IsRoundOver(int round, Dictionary<string, int[]> points);
}
