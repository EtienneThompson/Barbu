namespace Barbu.Core.Workflows
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Interfaces.Rounds;

    public interface IWorkflowFactory
    {
        public IWorkflow CreatePlayTrickWorkflow(
            IRound round,
            Dictionary<string, int[]> playerPoints,
            Hand[] playerHands,
            int startingPlayer,
            int roundNumber);

        public IWorkflow CreateRoundWorkflow(GameTypes gameType, List<IRound> rounds);
    }
}
