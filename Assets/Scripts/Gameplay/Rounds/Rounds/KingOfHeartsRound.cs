using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfHeartsRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart13", 40},
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 40;
    public override string Name => Constants.RoundNames.KingOfHeartsRound;

    public KingOfHeartsRound(RoundContext context)
    : base(context)
    {
    }

    public KingOfHeartsRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
