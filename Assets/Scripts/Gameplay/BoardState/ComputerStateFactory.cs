namespace Barbu.Gameplay.BoardState
{
    using Barbu.Interfaces.Rounds;

    public class ComputerStateFactory
    {
        public static GameState GetComputerStateFromSettings(
            IRound round,
            string id,
            Hand hand)
        {
            switch (Settings.ComputerDifficultyPreference)
            {
                case Settings.ComputerDifficulty.Hard:
                    return new HardComputerState(round, id, hand);
                case Settings.ComputerDifficulty.Normal:
                    return new NormalComputerState(round, id, hand);
                case Settings.ComputerDifficulty.Easy:
                default:
                    return new EasyComputerState(round, id, hand);
            }
        }
    }
}
