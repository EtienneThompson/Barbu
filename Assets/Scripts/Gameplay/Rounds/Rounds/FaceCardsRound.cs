using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCardsRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart11", 5 },
        { "Heart12", 5 },
        { "Heart13", 5 },
        { "Heart14", 5 },
        { "Diamond11", 5 },
        { "Diamond12", 5 },
        { "Diamond13", 5 },
        { "Diamond14", 5 },
        { "Spade11", 5 },
        { "Spade12", 5 },
        { "Spade13", 5 },
        { "Spade14", 5 },
        { "Club11", 5 },
        { "Club12", 5 },
        { "Club13", 5 },
        { "Club14", 5 },
    };
    public override int PointsPerPile => 0;
    public override int TotalPoints => 80;
    public override string Name => Constants.RoundNames.FaceCardsRound;

    public FaceCardsRound(RoundContext context)
    : base(context)
    {
    }

    public FaceCardsRound(RoundContext context, Round next)
    : base(context, next)
    {
    }
}
