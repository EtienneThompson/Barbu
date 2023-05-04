using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfHeartsRound : Round
{
    protected override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart13", 40},
    };
    protected override int PointsPerPile => 0;
    protected override int TotalPoints => 40;

    public KingOfHeartsRound(RoundContext context)
    : base(context)
    {
    }

    public KingOfHeartsRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
