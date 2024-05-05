namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class KingsRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Heart13", 10 },
        { "Diamond13", 10 },
        { "Spade13", 10 },
        { "Club13", 10 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 40;
        public override string Name => Constants.RoundNames.KingsRound;

        public KingsRound()
        {
        }
    }
}