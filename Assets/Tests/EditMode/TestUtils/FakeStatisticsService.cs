namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu;

    /// <summary>IStatisticsService double that records calls instead of touching real PlayerPrefs.</summary>
    public class FakeStatisticsService : IStatisticsService
    {
        public int GamesFinishedToReturn { get; set; }

        public int GamesWonToReturn { get; set; }

        public List<GameTypes> GamesPlayedIncrements { get; } = new();

        public List<GameTypes> GamesFinishedIncrements { get; } = new();

        public List<GameTypes> GamesWonIncrements { get; } = new();

        public int GetGamesFinished()
        {
            return this.GamesFinishedToReturn;
        }

        public int GetGamesWon()
        {
            return this.GamesWonToReturn;
        }

        public void IncrementGamesPlayed(GameTypes type)
        {
            this.GamesPlayedIncrements.Add(type);
        }

        public void IncrementGamesFinished(GameTypes type)
        {
            this.GamesFinishedIncrements.Add(type);
        }

        public void IncrementGamesWon(GameTypes type)
        {
            this.GamesWonIncrements.Add(type);
        }
    }
}
