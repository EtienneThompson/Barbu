namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

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
        public override string Name => Constants.RoundNames.TwosRound;

        public TwosRound(RoundContext context)
        : base(context)
        {
        }

        public TwosRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}