using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundContext
{
    private IRound current;

    public RoundContext()
    {
    }

    public void Next()
    {
        current.GoNext();
    }

    public void SetState(IRound current)
    {
        this.current = current;
    }

    public int CalculatePointsInPile(Card[] pile)
    {
        return this.current.CalculatePointsInPile(pile);
    }

    public int CalculateCurrentPoints(List<Card[]> piles)
    {
        return this.current.CalculatePointsInAllPiles(piles);
    }

    public bool IsRoundOver(int round, Dictionary<string, int[]> points)
    {
        return this.current.IsRoundOver(round, points);
    }
}
