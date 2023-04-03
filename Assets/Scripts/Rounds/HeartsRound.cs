using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsRound : Round
{
    public Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart2", 5},
        {"Heart3", 5},
        {"Heart4", 5},
        {"Heart5", 5},
        {"Heart6", 5},
        {"Heart7", 5},
        {"Heart8", 5},
        {"Heart9", 5},
        {"Heart10", 5},
        {"Heart11", 5},
        {"Heart12", 5},
        {"Heart13", 5},
        {"Heart14", 5},
    };
    public int PointsPerPile => 0;
    public int TotalPoints => 65;
    private RoundContext context;
    private Round nextState;

    public HeartsRound(RoundContext context)
    {
        this.context = context;
    }

    public HeartsRound(RoundContext context, Round next)
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
            totalPoints += this.CalculatePointsInPile(pile) + this.PointsPerPile;
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
