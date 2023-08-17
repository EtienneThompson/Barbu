using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixesRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart6", 5 },
        { "Diamond6", 5 },
        { "Spade6", 5 },
        { "Club6", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 65;
    public override string Name => Constants.RoundNames.SixesRound;

    public SixesRound(RoundContext context)
    : base(context)
    {
    }

    public SixesRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
