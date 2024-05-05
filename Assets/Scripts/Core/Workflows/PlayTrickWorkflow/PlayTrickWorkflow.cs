namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Interfaces.Rounds;
    using Barbu.Models.Workflows;

    public class PlayTrickWorkflow : BaseWorkflow<PlayTrickArguments>
    {
        private IRound round;
        private Dictionary<string, int[]> playerPoints;
        private int roundNumber;

        protected override Dictionary<string, IStep<PlayTrickArguments>> Steps => new Dictionary<string, IStep<PlayTrickArguments>>
        {
            [nameof(PlayCardStep)] = new PlayCardStep(),
            [nameof(HandleCardPlayedStep)] = new HandleCardPlayedStep(),
            [nameof(ResolveTrickStep)] = new ResolveTrickStep(),
        };

        public PlayTrickWorkflow(
            IRound round,
            Dictionary<string, int[]> playerPoints,
            Hand[] playerHands,
            int startingPlayer,
            int roundNumber)
            : base()
        {
            this.round = round;
            this.playerPoints = playerPoints;
            this.roundNumber = roundNumber;

            var gameStates = new GameState[4];
            gameStates[0] = new PlayerState(round, Constants.PlayerIds.Player1, playerHands[0]);
            gameStates[1] = ComputerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player2, playerHands[1]);
            gameStates[2] = ComputerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player3, playerHands[2]);
            gameStates[3] = ComputerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player4, playerHands[3]);

            this.currentStepName = nameof(PlayCardStep);

            this.Arguments = new StepArguments<PlayTrickArguments>
            {
                Data = new PlayTrickArguments
                {
                    gameStates = gameStates,
                    currentGameStateIndex = startingPlayer,
                    currentPile = new Pile(),
                    Round = round,
                    RoundNumber = roundNumber,
                    PlayerPoints = playerPoints,
                },
            };

            this.stateMachine.SetStartingSuit(string.Empty);
        }

        public string GetWinningPlayerId()
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Getting winning player...");
            return this.Arguments.Data.currentPile.GetHighestCard().playerId;
        }
    }
}