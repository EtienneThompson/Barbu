using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinesRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart9", 5 },
        { "Diamond9", 5 },
        { "Spade9", 5 },
        { "Club9", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 20;
    public override string Name => Constants.RoundNames.NinesRound;

    public NinesRound(RoundContext context)
    : base(context)
    {
    }

    public NinesRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
