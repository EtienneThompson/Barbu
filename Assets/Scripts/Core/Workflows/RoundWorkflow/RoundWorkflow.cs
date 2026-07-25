namespace Barbu.Core.Workflows.RoundWorkflow
{
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows;
    using Barbu.Gameplay.Rounds;
    using System.Collections.Generic;
    using Zenject;

    public class RoundWorkflow : BaseWorkflow<RoundArguments>
    {
        public class Factory : PlaceholderFactory<GameTypes, List<IRound>, RoundWorkflow>
        {
        }

        private readonly SetupRoundStep.Factory setupRoundStepFactory;
        private readonly PreRoundStep.Factory preRoundStepFactory;
        private readonly StartRoundStep.Factory startRoundStepFactory;
        private readonly CleanupRoundStep.Factory cleanupRoundStepFactory;
        private readonly NextRoundStep.Factory nextRoundStepFactory;
        private readonly CompleteGameStep.Factory completeGameStepFactory;
        private readonly ShowAdvertisementStep.Factory showAdvertisementStepFactory;

        protected override Dictionary<string, IStep<RoundArguments>> Steps => new Dictionary<string, IStep<RoundArguments>>
        {
            [nameof(SetupRoundStep)] = this.setupRoundStepFactory.Create(),
            [nameof(PreRoundStep)] = this.preRoundStepFactory.Create(),
            [nameof(StartRoundStep)] = this.startRoundStepFactory.Create(),
            [nameof(CleanupRoundStep)] = this.cleanupRoundStepFactory.Create(),
            [nameof(NextRoundStep)] = this.nextRoundStepFactory.Create(),
            [nameof(CompleteGameStep)] = this.completeGameStepFactory.Create(),
            [nameof(ShowAdvertisementStep)] = this.showAdvertisementStepFactory.Create(),
        };

        public RoundWorkflow(
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IAdvertisementController advertisementController,
            SetupRoundStep.Factory setupRoundStepFactory,
            PreRoundStep.Factory preRoundStepFactory,
            StartRoundStep.Factory startRoundStepFactory,
            CleanupRoundStep.Factory cleanupRoundStepFactory,
            NextRoundStep.Factory nextRoundStepFactory,
            CompleteGameStep.Factory completeGameStepFactory,
            ShowAdvertisementStep.Factory showAdvertisementStepFactory,
            GameTypes gameType,
            List<IRound> rounds)
            : base(eventsController, stateMachine, telemetryService)
        {
            this.setupRoundStepFactory = setupRoundStepFactory;
            this.preRoundStepFactory = preRoundStepFactory;
            this.startRoundStepFactory = startRoundStepFactory;
            this.cleanupRoundStepFactory = cleanupRoundStepFactory;
            this.nextRoundStepFactory = nextRoundStepFactory;
            this.completeGameStepFactory = completeGameStepFactory;
            this.showAdvertisementStepFactory = showAdvertisementStepFactory;
            this.currentStepName = nameof(SetupRoundStep);
            this.Arguments = new StepArguments<RoundArguments>(this)
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

            advertisementController.RequestToShowInterstitial();
        }

        public Dictionary<string, int[]> GetPlayerPoints()
        {
            return this.Arguments.Data.PlayerPoints;
        }

        public int GetCurrentRoundIndex()
        {
            return this.Arguments.Data.CurrentRoundIndex;
        }
    }
}
