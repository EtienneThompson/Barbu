namespace Barbu.Gameplay.BoardState
{
    public class ComputerStateFactory
    {
        public static GameState GetComputerStateFromSettings(
            GameStateContext context,
            string id,
            Hand hand,
            GameState next = null)
        {
            switch (Settings.ComputerDifficultyPreference)
            {
                case Settings.ComputerDifficulty.Hard:
                    return new HardComputerState(context, next, id, hand);
                case Settings.ComputerDifficulty.Normal:
                    return new NormalComputerState(context, next, id, hand);
                case Settings.ComputerDifficulty.Easy:
                default:
                    return new EasyComputerState(context, next, id, hand);
            }
        }
    }
}
