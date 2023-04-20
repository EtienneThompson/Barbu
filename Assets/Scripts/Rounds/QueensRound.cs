using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueensRound : Round
{
    protected override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart12", 10},
        {"Diamond12", 10},
        {"Spade12", 10},
        {"Club12", 10},
    };
    protected override int PointsPerPile => 0;
    protected override int TotalPoints => 40;

    public QueensRound(RoundContext context)
    : base(context)
    {
    }

    public QueensRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
