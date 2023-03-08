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

    public int CalculatePointsInPiles(List<Card[]> piles)
    {
        int totalPoints = 0;
        foreach (var pile in piles)
        {
            foreach (var card in pile)
            {
                Debug.Log(card.GetName());
                if (this.PointMapping.TryGetValue(card.GetName(), out var points))
                {
                    totalPoints += points;
                }
            }

            totalPoints += this.PointsPerPile;
        }

        return totalPoints;
    }
}
