namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.UI.Controllers;

    /// <summary>IRoundOverlayController double that records calls instead of touching TextMeshPro/coroutines.</summary>
    public class FakeRoundOverlayController : IRoundOverlayController
    {
        public List<(string roundName, GameTypes gameType)> DisplayedRounds { get; } = new();

        public List<string[]> DisplayedWinners { get; } = new();

        public int ShowRoundOverMessageCallCount { get; private set; }

        public int HideTextCallCount { get; private set; }

        public List<bool> ActiveStates { get; } = new();

        public void DisplayRound(string roundName, GameTypes gameType)
        {
            this.DisplayedRounds.Add((roundName, gameType));
        }

        public void DisplayWinner(string[] winningPlayers)
        {
            this.DisplayedWinners.Add(winningPlayers);
        }

        public void ShowRoundOverMessage()
        {
            this.ShowRoundOverMessageCallCount++;
        }

        public void HideText()
        {
            this.HideTextCallCount++;
        }

        public void SetActive(bool active)
        {
            this.ActiveStates.Add(active);
        }
    }
}
