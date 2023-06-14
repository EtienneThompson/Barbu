using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvensRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart2", 5 },
        { "Heart4", 5 },
        { "Heart6", 5 },
        { "Heart8", 5 },
        { "Heart10", 5 },
        { "Heart12", 5 },
        { "Heart14", 5 },
        { "Diamond2", 5 },
        { "Diamond4", 5 },
        { "Diamond6", 5 },
        { "Diamond8", 5 },
        { "Diamond10", 5 },
        { "Diamond12", 5 },
        { "Diamond14", 5 },
        { "Spade2", 5 },
        { "Spade4", 5 },
        { "Spade6", 5 },
        { "Spade8", 5 },
        { "Spade10", 5 },
        { "Spade12", 5 },
        { "Spade14", 5 },
        { "Club2", 5 },
        { "Club4", 5 },
        { "Club6", 5 },
        { "Club8", 5 },
        { "Club10", 5 },
        { "Club12", 5 },
        { "Club14", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 140;
    public override string Name => nameof(EvensRound);

    public EvensRound(RoundContext context)
    : base(context)
    {
    }

    public EvensRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
