namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class HeartsRound : Round
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
        {"Heart12", 5},
        {"Heart13", 5},
        {"Heart14", 5},
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 65;
        public override string Name => Constants.RoundNames.HeartsRound;

        public HeartsRound(RoundContext context)
        : base(context)
        {
        }

        public HeartsRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}
