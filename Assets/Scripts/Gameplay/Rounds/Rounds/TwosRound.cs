using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwosRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart2", 5 },
        { "Diamond2", 5 },
        { "Spade2", 5 },
        { "Club2", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 20;
    public override string Name => nameof(TwosRound);

    public TwosRound(RoundContext context)
    : base(context)
    {
    }

    public TwosRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
