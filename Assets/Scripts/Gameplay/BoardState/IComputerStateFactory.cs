namespace Barbu
{
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Interfaces.Rounds;

    public interface IComputerStateFactory
    {
        GameState GetComputerStateFromSettings(
            IRound round,
            string id,
            Hand hand);
    }
}
