namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.UI.Controllers;

    /// <summary>
    /// IInGamePointsController double that records calls, so tests can assert on
    /// round/point/visibility updates without a real scene overlay.
    /// </summary>
    public class FakeInGamePointsController : IInGamePointsController
    {
        public List<string> RoundNamesSet { get; } = new();

        public int ResetRoundNameCallCount { get; private set; }

        public Dictionary<string, int> PlayerPoints { get; } = new();

        public int ResetPointsCallCount { get; private set; }

        public List<bool> ActiveStates { get; } = new();

        public void SetRoundName(string roundName)
        {
            this.RoundNamesSet.Add(roundName);
        }

        public void ResetRoundName()
        {
            this.ResetRoundNameCallCount++;
        }

        public void UpdatePlayerPoints(string player, int points)
        {
            this.PlayerPoints.TryGetValue(player, out var current);
            this.PlayerPoints[player] = current + points;
        }

        public void ResetPoints()
        {
            this.ResetPointsCallCount++;
            this.PlayerPoints.Clear();
        }

        public void SetActive(bool active)
        {
            this.ActiveStates.Add(active);
        }
    }
}
