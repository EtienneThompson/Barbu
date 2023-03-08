using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundContext
{
    private Round current;

    public RoundContext()
    {
    }

    public void Next()
    {
        current.GoNext();
    }

    public void SetState(Round current)
    {
        this.current = current;
    }

    public int CalculateCurrentPoints(List<Card[]> piles)
    {
        return this.current.CalculatePointsInPiles(piles);
    }
}
