namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class TensRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart10", 5 },
        { "Diamond10", 5 },
        { "Spade10", 5 },
        { "Club10", 5 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 20;
        public override string Name => Constants.RoundNames.TensRound;

        public TensRound()
        {
        }
    }
}
