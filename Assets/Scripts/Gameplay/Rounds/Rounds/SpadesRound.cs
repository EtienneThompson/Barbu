namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class SpadesRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Spade2", 5 },
        { "Spade3", 5 },
        { "Spade4", 5 },
        { "Spade5", 5 },
        { "Spade6", 5 },
        { "Spade7", 5 },
        { "Spade8", 5 },
        { "Spade9", 5 },
        { "Spade10", 5 },
        { "Spade11", 5 },
        { "Spade12", 5 },
        { "Spade13", 5 },
        { "Spade14", 5 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 65;
        public override string Name => Constants.RoundNames.SpadesRound;

        public SpadesRound()
        {
        }
    }
}