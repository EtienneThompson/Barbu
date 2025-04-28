namespace Barbu.Models.Workflows
{
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Interfaces.Rounds;
    using System.Collections.Generic;

    public class PlayTrickArguments
    {
        public GameState[] gameStates { get; set; }
        
        public int currentGameStateIndex { get; set; }

        public Pile currentPile { get; set; }

        public IRound Round { get; set; }

        public int RoundNumber { get; set; }

        public Dictionary<string, int[]> PlayerPoints { get; set; }
    }
}