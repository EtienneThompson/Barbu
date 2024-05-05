namespace Barbu
{
    using Barbu.Core.Workflows;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Interfaces.Rounds;
    using System.Collections.Generic;

    public class RoundWorkflow : BaseWorkflow<RoundArguments>
    {
        protected override Dictionary<string, IStep<RoundArguments>> Steps => new Dictionary<string, IStep<RoundArguments>>
        {
            [nameof(SetupRoundStep)] = new SetupRoundStep(),
            [nameof(PreRoundStep)] = new PreRoundStep(),
            [nameof(StartRoundStep)] = new StartRoundStep(),
            [nameof(CleanupRoundStep)] = new CleanupRoundStep(),
            [nameof(NextRoundStep)] = new NextRoundStep(),
            [nameof(CompleteGameStep)] = new CompleteGameStep(),
        };

        public RoundWorkflow(Statistics.GameTypes gameType, List<IRound> rounds)
        {
            this.currentStepName = nameof(SetupRoundStep);
            this.Arguments = new Models.Workflows.StepArguments<RoundArguments>
            {
                Data = new RoundArguments
                {
                    GameType = gameType,
                    Rounds = rounds,
                    CurrentRoundIndex = 0,
                    PlayerPoints = new Dictionary<string, int[]>
                    {
                        [Constants.PlayerIds.Player1] = new int[rounds.Count],
                        [Constants.PlayerIds.Player2] = new int[rounds.Count],
                        [Constants.PlayerIds.Player3] = new int[rounds.Count],
                        [Constants.PlayerIds.Player4] = new int[rounds.Count],
                    },
                    TricksPlayed = 0,
                }
            };
        }
    }
}
