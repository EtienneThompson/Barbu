namespace Barbu.Models.Workflows
{
    using Barbu.Gameplay.BoardState;

    public class PlayTrickArguments
    {
        public GameState[] gameStates { get; set; }
        
        public int currentGameStateIndex { get; set; }

        public int cardsPlayed { get; set; } = 0;
    }
}