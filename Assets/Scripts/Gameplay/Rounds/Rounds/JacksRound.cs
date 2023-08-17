using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacksRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart11", 10 },
        { "Diamond11", 10 },
        { "Spade11", 10 },
        { "Club11", 10 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 20;
    public override string Name => Constants.RoundNames.JacksRound;

    public JacksRound(RoundContext context)
    : base(context)
    {
    }

    public JacksRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
