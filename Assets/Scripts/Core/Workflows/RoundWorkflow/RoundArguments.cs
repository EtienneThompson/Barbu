namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Collections.Generic;
    using System.Linq;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;

    public class RoundArguments
    {
        public GameTypes GameType { get; set; }

        public List<IRound> Rounds { get; set; }

        public int CurrentRoundIndex { get; set; }

        public Hand[] Hands { get; set; }

        public Dictionary<string, int[]> PlayerPoints { get; set; }

        public int TricksPlayed { get; set; }

        public PlayTrickWorkflow PlayTrickWorkflow { get; set; }

        public IComputerStateFactory ComputerStateFactory { get; set; }

        public IRound GetCurrentRound()
        {
            return this.Rounds[this.CurrentRoundIndex];
        }

        public string[] GetWinningPlayerIds()
        {
            Dictionary<string, int> playerPointsSum = new Dictionary<string, int>
            {
                [Constants.PlayerIds.Player1] = this.PlayerPoints[Constants.PlayerIds.Player1].Sum(),
                [Constants.PlayerIds.Player2] = this.PlayerPoints[Constants.PlayerIds.Player2].Sum(),
                [Constants.PlayerIds.Player3] = this.PlayerPoints[Constants.PlayerIds.Player3].Sum(),
                [Constants.PlayerIds.Player4] = this.PlayerPoints[Constants.PlayerIds.Player4].Sum(),
            };

            var minPoints = playerPointsSum.Min(selector => selector.Value);

            return playerPointsSum
                .Where(playerPoints => playerPoints.Value == minPoints)
                .Select(points => points.Key)
                .ToArray();
        }
    }
}
