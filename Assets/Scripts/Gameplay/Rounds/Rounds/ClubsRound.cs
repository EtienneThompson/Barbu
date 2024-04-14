namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Gameplay.Rounds;

    public class ClubsRound : Round
    {
        public override Dictionary<string, int> PointMapping => new Dictionary<string, int>
    {
        { "Club2", 5 },
        { "Club3", 5 },
        { "Club4", 5 },
        { "Club5", 5 },
        { "Club6", 5 },
        { "Club7", 5 },
        { "Club8", 5 },
        { "Club9", 5 },
        { "Club10", 5 },
        { "Club11", 5 },
        { "Club12", 5 },
        { "Club13", 5 },
        { "Club14", 5 },
    };
        public override int PointsPerPile => 0;
        public override int TotalPoints => 65;
        public override string Name => Constants.RoundNames.ClubsRound;

        public ClubsRound(RoundContext context)
        : base(context)
        {
        }

        public ClubsRound(RoundContext context, Round next)
        : base(context, next)
        {
        }
    }
}
