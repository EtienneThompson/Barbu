using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoursRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart4", 5 },
        { "Diamond4", 5 },
        { "Spade4", 5 },
        { "Club4", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 20;
    public override string Name => Constants.RoundNames.FoursRound;

    public FoursRound(RoundContext context)
    : base(context)
    {
    }

    public FoursRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
