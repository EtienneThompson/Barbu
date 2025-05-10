namespace Barbu.Core.Workflows
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;

    public interface IWorkflowFactory
    {
        IWorkflow CreatePlayTrickWorkflow(
            IRound round,
            Dictionary<string, int[]> playerPoints,
            Hand[] playerHands,
            int startingPlayer,
            int roundNumber);

        RoundWorkflow.RoundWorkflow CreateTraditionalRoundWorkflow();

        RoundWorkflow.RoundWorkflow CreateSingleRoundWorkflow(string roundType);

        RoundWorkflow.RoundWorkflow CreateChaosRoundWorkflow();
    }
}
