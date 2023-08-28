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
            case Settings.ComputerDifficulty.Easy:
            default:
                return new EasyComputerState(context, next, id, hand);
        }
    }
}
