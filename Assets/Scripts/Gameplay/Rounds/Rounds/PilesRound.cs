namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class PilesRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>();
        public override int PointsPerPile => 5;
        public override int TotalPoints => 65;
        public override string Name => Constants.RoundNames.PilesRound;

        public PilesRound(RoundContext context)
        : base(context)
        {
        }

        public PilesRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}