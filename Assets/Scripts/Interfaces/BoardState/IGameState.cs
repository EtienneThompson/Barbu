namespace Barbu.Interfaces.BoardState
{
    using Barbu.Gameplay.BoardState;

    public interface IGameState
    {
        string PlayerId { get; }

        void Start();
        void CleanUp();
    }
}