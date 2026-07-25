namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Collections.Generic;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;
    using Zenject;

    public class PlayTrickWorkflow : BaseWorkflow<PlayTrickArguments>
    {
        // startingPlayer and roundNumber are both int, so Zenject matches them to
        // the constructor's (..., int startingPlayer, int roundNumber) params by
        // position among same-typed args, in the order passed to Create() - keep
        // this call's argument order in sync with that constructor's parameter order.
        public class Factory : PlaceholderFactory<IRound, Dictionary<string, int[]>, Hand[], int, int, PlayTrickWorkflow>
        {
        }

        private readonly IComputerStateFactory computerStateFactory;
        private readonly PlayCardStep.Factory playCardStepFactory;
        private readonly HandleCardPlayedStep.Factory handleCardPlayedStepFactory;
        private readonly ResolveTrickStep.Factory resolveTrickStepFactory;

        protected override Dictionary<string, IStep<PlayTrickArguments>> Steps => new Dictionary<string, IStep<PlayTrickArguments>>
        {
            [nameof(PlayCardStep)] = this.playCardStepFactory.Create(),
            [nameof(HandleCardPlayedStep)] = this.handleCardPlayedStepFactory.Create(),
            [nameof(ResolveTrickStep)] = this.resolveTrickStepFactory.Create(),
        };

        public PlayTrickWorkflow(
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IComputerStateFactory computerStateFactory,
            PlayCardStep.Factory playCardStepFactory,
            HandleCardPlayedStep.Factory handleCardPlayedStepFactory,
            ResolveTrickStep.Factory resolveTrickStepFactory,
            IRound round,
            Dictionary<string, int[]> playerPoints,
            Hand[] playerHands,
            int startingPlayer,
            int roundNumber)
            : base(eventsController, stateMachine, telemetryService)
        {
            this.computerStateFactory = computerStateFactory;
            this.playCardStepFactory = playCardStepFactory;
            this.handleCardPlayedStepFactory = handleCardPlayedStepFactory;
            this.resolveTrickStepFactory = resolveTrickStepFactory;
            var gameStates = new GameState[4]; 
            if (!this.stateMachine.IsAutoPlayMode())
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

            this.Arguments = new StepArguments<PlayTrickArguments>(this)
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