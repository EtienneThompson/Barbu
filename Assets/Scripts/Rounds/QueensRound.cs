using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueensRound : Round
{
    public Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart12", 10},
        {"Diamond12", 10},
        {"Spade12", 10},
        {"Club12", 10},
    };
    public int PointsPerPile => 0;
    public int TotalPoints => 40;
    private RoundContext context;
    private Round nextState;

    public QueensRound(RoundContext context)
    {
        this.context = context;
    }

    public QueensRound(RoundContext context, Round next)
    {
        this.context = context;
        this.nextState = next;
    }

    public void GoNext()
    {
        if (this.nextState == null)
        {
            throw new Exception("No next state set.");
        }

        this.context.SetState(this.nextState);
    }

    public void SetNextState(Round next)
    {
        this.nextState = next;
    }

    public int CalculatePointsInPile(Card[] pile)
    {
        int totalPoints = 0;
        foreach (var card in pile)
        {
            if (this.PointMapping.TryGetValue(card.GetName(), out var points))
            {
                totalPoints += points;
            }
        }

        return totalPoints;
    }

    public int CalculatePointsInAllPiles(List<Card[]> piles)
    {
        int totalPoints = 0;
        foreach (var pile in piles)
        {
            totalPoints += this.CalculatePointsInPile(pile);
        }

        return totalPoints;
    }

    public bool IsRoundOver(int round, Dictionary<string, int[]> playerPoints)
    {
        int totalPoints = 0;
        foreach (var key in playerPoints.Keys)
        {
            totalPoints += playerPoints[key][round];
        }

        return totalPoints == this.TotalPoints;
    }
}
