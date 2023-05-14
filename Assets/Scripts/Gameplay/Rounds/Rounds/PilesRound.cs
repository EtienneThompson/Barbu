using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilesRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>();
    public override int PointsPerPile => 5;
    public override int TotalPoints => 65;
    public override string Name => nameof(PilesRound);

    public PilesRound(RoundContext context)
    : base(context)
    {
    }

    public PilesRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
