namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;
    using Barbu.UI.Controllers;

    public class PlayTrickWorkflow : BaseWorkflow<PlayTrickArguments>
    {
        private RoundContext roundContext;
        private InGamePointsController inGamePointsController;
        private Dictionary<string, int[]> playerPoints;
        private int roundNumber;

        protected override Dictionary<string, IStep<PlayTrickArguments>> Steps => new Dictionary<string, IStep<PlayTrickArguments>>
        {
            [nameof(PlayCardStep)] = new PlayCardStep(),
            [nameof(HandleCardPlayedStep)] = new HandleCardPlayedStep(),
            [nameof(ResolveTrickStep)] = new ResolveTrickStep(),
        };

        public PlayTrickWorkflow(
            RoundContext roundContext,
            InGamePointsController inGamePointsController,
            Dictionary<string, int[]> playerPoints,
            Hand[] playerHands,
            int startingPlayer,
            int roundNumber)
            : base()
        {
            this.roundContext = roundContext;
            this.inGamePointsController = inGamePointsController;
            this.playerPoints = playerPoints;
            this.roundNumber = roundNumber;

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
                    currentPile = new Pile(),
                },
            };

            this.stateMachine.SetStartingSuit(string.Empty);
        }

        protected override Task OnWorkflowEnd()
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Handling end of workflow...");
            var pointsInPile = this.roundContext.CalculatePointsInPile(this.Arguments.Data.currentPile);
            var playerId = this.GetWinningPlayerId();

            this.telemetryService.LogInfo($"[PlayTrickWorkflow] Winning player: {playerId}");
            this.inGamePointsController.UpdatePlayerPoints(playerId, pointsInPile);

            this.playerPoints[playerId][this.roundNumber] += pointsInPile;

            return Task.CompletedTask;
        }

        public string GetWinningPlayerId()
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Getting winning player...");
            return this.Arguments.Data.currentPile.GetHighestCard().playerId;
        }
    }
}