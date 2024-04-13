namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class OddsRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart3", 5 },
        { "Heart5", 5 },
        { "Heart7", 5 },
        { "Heart9", 5 },
        { "Heart11", 5 },
        { "Heart13", 5 },
        { "Diamond3", 5 },
        { "Diamond5", 5 },
        { "Diamond7", 5 },
        { "Diamond9", 5 },
        { "Diamond11", 5 },
        { "Diamond13", 5 },
        { "Spade3", 5 },
        { "Spade5", 5 },
        { "Spade7", 5 },
        { "Spade9", 5 },
        { "Spade11", 5 },
        { "Spade13", 5 },
        { "Club3", 5 },
        { "Club5", 5 },
        { "Club7", 5 },
        { "Club9", 5 },
        { "Club11", 5 },
        { "Club13", 5 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 120;
        public override string Name => Constants.RoundNames.OddsRound;

        public OddsRound(RoundContext context)
        : base(context)
        {
        }

        public OddsRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}
