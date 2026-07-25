namespace Barbu
{
    public interface IStatisticsService
    {
        int GetGamesFinished();

        int GetGamesWon();

        void IncrementGamesPlayed(GameTypes type);

        void IncrementGamesFinished(GameTypes type);

        void IncrementGamesWon(GameTypes type);
    }
}
