using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilesRound : Round
{
    protected override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
    };
    protected override int PointsPerPile => 5;
    protected override int TotalPoints => 65;

    public PilesRound(RoundContext context)
    : base(context)
    {
    }

    public PilesRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
