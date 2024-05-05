namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class DiamondsRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Diamond2", 5 },
        { "Diamond3", 5 },
        { "Diamond4", 5 },
        { "Diamond5", 5 },
        { "Diamond6", 5 },
        { "Diamond7", 5 },
        { "Diamond8", 5 },
        { "Diamond9", 5 },
        { "Diamond10", 5 },
        { "Diamond11", 5 },
        { "Diamond12", 5 },
        { "Diamond13", 5 },
        { "Diamond14", 5 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 65;
        public override string Name => Constants.RoundNames.DiamondRound;

        public DiamondsRound()
        {
        }
    }
}
