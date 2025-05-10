namespace Barbu
{
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;

    public interface IComputerStateFactory
    {
        GameState GetComputerStateFromSettings(
            IRound round,
            string id,
            Hand hand);
    }
}
