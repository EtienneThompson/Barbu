namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class EightsRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart8", 10 },
        { "Diamond8", 10 },
        { "Spade8", 10 },
        { "Club8", 10 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 40;
        public override string Name => Constants.RoundNames.EightsRound;

        public EightsRound(RoundContext context)
        : base(context)
        {
        }

        public EightsRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}