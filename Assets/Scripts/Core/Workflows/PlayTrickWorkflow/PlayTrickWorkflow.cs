namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Collections.Generic;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;

    public class PlayTrickWorkflow : BaseWorkflow<PlayTrickArguments>
    {
        private readonly IComputerStateFactory computerStateFactory;

        protected override Dictionary<string, IStep<PlayTrickArguments>> Steps => new Dictionary<string, IStep<PlayTrickArguments>>
        {
            [nameof(PlayCardStep)] = new PlayCardStep(),
            [nameof(HandleCardPlayedStep)] = new HandleCardPlayedStep(),
            [nameof(ResolveTrickStep)] = new ResolveTrickStep(),
        };

        public PlayTrickWorkflow(
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IComputerStateFactory computerStateFactory,
            IRound round,
            Dictionary<string, int[]> playerPoints,
            Hand[] playerHands,
            int startingPlayer,
            int roundNumber)
            : base(eventsController, stateMachine, telemetryService)
        {
            this.computerStateFactory = computerStateFactory;
            var gameStates = new GameState[4]; 
            if (!this.stateMachine.AutoPlayCards)
            {
                gameStates[0] = new PlayerState(stateMachine, telemetryService, round, Constants.PlayerIds.Player1, playerHands[0]);
            }
            else
            {
                gameStates[0] = this.computerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player1, playerHands[0]);
            }

            gameStates[1] = this.computerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player2, playerHands[1]);
            gameStates[2] = this.computerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player3, playerHands[2]);
            gameStates[3] = this.computerStateFactory.GetComputerStateFromSettings(round, Constants.PlayerIds.Player4, playerHands[3]);

            this.currentStepName = nameof(PlayCardStep);

            this.Arguments = new StepArguments<PlayTrickArguments>
            {
                Data = new PlayTrickArguments
                {
                    gameStates = gameStates,
                    currentGameStateIndex = startingPlayer,
                    currentPile = new Pile(stateMachine, eventsController),
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