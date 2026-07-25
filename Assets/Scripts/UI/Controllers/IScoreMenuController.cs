namespace Barbu.UI.Controllers
{
    using System.Collections.Generic;

    public interface IScoreMenuController
    {
        void DisplayScores(
            Dictionary<string, int[]> scores,
            int currentRoundIndex,
            bool isPositiveRound = false,
            bool skipAnimations = false);
    }
}
