namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;

    public class PlayTrickWorkflow : BaseWorkflow<PlayTrickArguments>
    {
        protected override Dictionary<string, IStep<PlayTrickArguments>> Steps => new Dictionary<string, IStep<PlayTrickArguments>>
        {
            [nameof(PlayCardStep)] = new PlayCardStep(),
            [nameof(HandleCardPlayedStep)] = new HandleCardPlayedStep(),
            [nameof(ResolveTrickStep)] = new ResolveTrickStep(),
        };

        public PlayTrickWorkflow(RoundContext roundContext, Hand[] playerHands, int startingPlayer)
            : base()
        {
            var gameStateContext = new GameStateContext(roundContext);
            var gameStates = new GameState[4];
            gameStates[0] = new PlayerState(gameStateContext, Constants.PlayerIds.Player1, playerHands[0]);
            gameStates[1] = ComputerStateFactory.GetComputerStateFromSettings(gameStateContext, Constants.PlayerIds.Player2, playerHands[1]);
            gameStates[2] = ComputerStateFactory.GetComputerStateFromSettings(gameStateContext, Constants.PlayerIds.Player3, playerHands[2]);
            gameStates[3] = ComputerStateFactory.GetComputerStateFromSettings(gameStateContext, Constants.PlayerIds.Player4, playerHands[3]);

            this.currentStepName = nameof(PlayCardStep);

            this.Arguments = new StepArguments<PlayTrickArguments>
            {
                Data = new PlayTrickArguments
                {
                    gameStates = gameStates,
                    currentGameStateIndex = startingPlayer,
                },
            };
        }
    }
}