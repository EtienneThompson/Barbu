namespace Barbu.Core.Workflows.RoundWorkflow
{
    using Barbu.Core.Workflows;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Interfaces.Rounds;
    using Barbu.Models.Workflows;
    using System.Collections.Generic;
    using UnityEngine;

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
            [nameof(ShowAdvertisementStep)] = new ShowAdvertisementStep(),
        };

        public RoundWorkflow(
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IComputerStateFactory computerStateFactory,
            GameTypes gameType,
            List<IRound> rounds)
            : base(eventsController, stateMachine, telemetryService)
        {
            this.currentStepName = nameof(SetupRoundStep);
            this.Arguments = new StepArguments<RoundArguments>
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
                    ComputerStateFactory = computerStateFactory,
                }
            };

            var gameBoard = GameObject.Find(Constants.GameObjects.GameBoard);
            var advertisementController = gameBoard.GetComponent<AdvertisementController>();
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
