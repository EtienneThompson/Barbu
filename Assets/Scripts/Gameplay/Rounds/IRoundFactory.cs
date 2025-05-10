namespace Barbu.Gameplay.Rounds
{
    using Barbu.Core.Workflows.RoundWorkflow;

    public interface IRoundFactory
    {
        RoundWorkflow CreateTraditionalRoundWorkflow();

        RoundWorkflow CreateSingleRoundWorkflow(string roundType);

        RoundWorkflow CreateChaosRoundWorkflow();
    }
}
