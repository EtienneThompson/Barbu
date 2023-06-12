using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcesRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart14", 10 },
        { "Diamond14", 10 },
        { "Spade14", 10 },
        { "Club14", 10 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 40;
    public override string Name => nameof(AcesRound);

    public AcesRound(RoundContext context)
    : base(context)
    {
    }

    public AcesRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
