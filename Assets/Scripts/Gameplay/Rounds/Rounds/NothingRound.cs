namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class NothingRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart2", 5},
        {"Heart3", 5},
        {"Heart4", 5},
        {"Heart5", 5},
        {"Heart6", 5},
        {"Heart7", 5},
        {"Heart8", 5},
        {"Heart9", 5},
        {"Heart10", 5},
        {"Heart11", 5},
        {"Heart12", 15},
        {"Heart13", 45},
        {"Heart14", 5},
        {"Diamond12", 10},
        {"Spade12", 10},
        {"Club12", 10}
    };
        public override int PointsPerPile => 5;
        public override int TotalPoints => 210;
        public override string Name => Constants.RoundNames.NothingRound;

        public NothingRound(RoundContext context)
        : base(context)
        {
        }

        public NothingRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}
