using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsRound : Round
{
    public Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart02", 5},
        {"Heart03", 5},
        {"Heart04", 5},
        {"Heart05", 5},
        {"Heart06", 5},
        {"Heart07", 5},
        {"Heart08", 5},
        {"Heart09", 5},
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
}
