using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreesRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart3", 5 },
        { "Diamond3", 5 },
        { "Spade3", 5 },
        { "Club3", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 20;
    public override string Name => nameof(ThreesRound);

    public ThreesRound(RoundContext context)
    : base(context)
    {
    }

    public ThreesRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
