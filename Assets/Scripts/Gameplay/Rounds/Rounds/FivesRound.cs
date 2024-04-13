namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class FivesRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart5", 5 },
        { "Diamond5", 5 },
        { "Spade5", 5 },
        { "Club5", 5 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 20;
        public override string Name => Constants.RoundNames.FivesRound;

        public FivesRound(RoundContext context)
        : base(context)
        {
        }

        public FivesRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}