using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Round
{
    Dictionary<string, int> PointMapping { get; }
    int PointsPerPile { get; }
    int TotalPoints { get; }

    void GoNext();
    int CalculatePointsInPile(Card[] pile);
    int CalculatePointsInAllPiles(List<Card[]> piles);
    bool IsRoundOver(Dictionary<string, int> points);
}
