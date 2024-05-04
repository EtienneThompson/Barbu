namespace Barbu.Models.Workflows
{
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using System.Collections.Generic;

    public class PlayTrickArguments
    {
        public GameState[] gameStates { get; set; }
        
        public int currentGameStateIndex { get; set; }

        public Pile currentPile { get; set; }
    }
}