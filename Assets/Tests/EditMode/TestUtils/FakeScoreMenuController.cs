namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.UI.Controllers;

    /// <summary>IScoreMenuController double that records the last DisplayScores call instead of animating UI.</summary>
    public class FakeScoreMenuController : IScoreMenuController
    {
        public int DisplayScoresCallCount { get; private set; }

        public Dictionary<string, int[]> LastScores { get; private set; }

        public int LastCurrentRoundIndex { get; private set; }

        public bool LastIsPositiveRound { get; private set; }

        public void DisplayScores(
            Dictionary<string, int[]> scores,
            int currentRoundIndex,
            bool isPositiveRound = false,
            bool skipAnimations = false)
        {
            this.DisplayScoresCallCount++;
            this.LastScores = scores;
            this.LastCurrentRoundIndex = currentRoundIndex;
            this.LastIsPositiveRound = isPositiveRound;
        }
    }
}
