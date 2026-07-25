namespace Barbu
{
    using Barbu.Gameplay;

    public interface IGameBoard
    {
        Hand[] SetupRound();

        void CleanupRound();
    }
}
