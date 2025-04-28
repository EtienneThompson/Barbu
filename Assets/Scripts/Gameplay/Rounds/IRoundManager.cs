namespace Barbu.Interfaces.Rounds
{
    using Barbu.Gameplay;

    public interface IRoundManager
    {
        void PreRound();
        void StartRound();
        void CleanupRound();
        void NextRound(Hand[] hands);
        void Destroy();
    }
}
