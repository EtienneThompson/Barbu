using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SevensRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart7", 5 },
        { "Diamond7", 5 },
        { "Spade7", 5 },
        { "Club7", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 65;
    public override string Name => nameof(SevensRound);

    public SevensRound(RoundContext context)
    : base(context)
    {
    }

    public SevensRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
