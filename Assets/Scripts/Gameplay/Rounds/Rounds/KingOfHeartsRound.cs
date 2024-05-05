namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class KingOfHeartsRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        {"Heart13", 40},
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 40;
        public override string Name => Constants.RoundNames.KingOfHeartsRound;

        public KingOfHeartsRound()
        {
        }
    }
}