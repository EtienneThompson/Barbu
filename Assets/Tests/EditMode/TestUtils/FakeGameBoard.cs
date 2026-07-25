namespace Barbu.Tests.EditMode.TestUtils
{
    using Barbu.Gameplay;

    /// <summary>IGameBoard double that records calls instead of dealing real cards.</summary>
    public class FakeGameBoard : IGameBoard
    {
        public int SetupRoundCallCount { get; private set; }

        public int CleanupRoundCallCount { get; private set; }

        public Hand[] HandsToReturn { get; set; } = new Hand[0];

        public Hand[] SetupRound()
        {
            this.SetupRoundCallCount++;
            return this.HandsToReturn;
        }

        public void CleanupRound()
        {
            this.CleanupRoundCallCount++;
        }
    }
}
