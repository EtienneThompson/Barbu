using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Round
{
    Dictionary<string, int> PointMapping { get; }
    int PointsPerPile { get; }

    void GoNext();
    int CalculatePointsInPiles(List<Card[]> piles);
}
