namespace Barbu.Interfaces.BoardState
{
    public interface IGameState
    {
        string PlayerId { get; }

        void Start();
        void CleanUp();
    }
}