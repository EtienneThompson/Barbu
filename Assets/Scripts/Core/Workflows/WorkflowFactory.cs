namespace Barbu.Core.Workflows
{
    using System;
    using System.Collections.Generic;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;
    using Barbu.Gameplay.Rounds.Rounds;
    using UnityEngine;

    public class WorkflowFactory : IWorkflowFactory
    {
        private readonly ITelemetryService telemetryService;
        private readonly PlayTrickWorkflow.PlayTrickWorkflow.Factory playTrickWorkflowFactory;
        private readonly RoundWorkflow.RoundWorkflow.Factory roundWorkflowFactory;

        public WorkflowFactory(
            ITelemetryService telemetryService,
            PlayTrickWorkflow.PlayTrickWorkflow.Factory playTrickWorkflowFactory,
            RoundWorkflow.RoundWorkflow.Factory roundWorkflowFactory)
        {
            this.telemetryService = telemetryService;
            this.playTrickWorkflowFactory = playTrickWorkflowFactory;
            this.roundWorkflowFactory = roundWorkflowFactory;
        }

        public IWorkflow CreatePlayTrickWorkflow(IRound round, Dictionary<string, int[]> playerPoints, Hand[] playerHands, int startingPlayer, int roundNumber)
        {
            return this.playTrickWorkflowFactory.Create(round, playerPoints, playerHands, startingPlayer, roundNumber);
        }

        public RoundWorkflow.RoundWorkflow CreateTraditionalRoundWorkflow()
        {
            var heartsRound = new HeartsRound();
            var queensRound = new QueensRound();
            var kohRound = new KingOfHeartsRound();
            var pilesRound = new PilesRound();
            var nothingRound = new NothingRound();
            var everythingRound = new EverythingRound();
            var roundList = new List<IRound>()
            {
                heartsRound,
                queensRound,
                kohRound,
                pilesRound,
                nothingRound,
                everythingRound,
            };

            return this.CreateRoundWorkflow(GameTypes.Traditional, roundList);
        }

        public RoundWorkflow.RoundWorkflow CreateSingleRoundWorkflow(string roundType)
        {
            Round round;
            switch (roundType)
            {
                case Constants.SingleRoundManager.Hearts:
                    round = new HeartsRound();
                    break;
                case Constants.SingleRoundManager.Queens:
                    round = new QueensRound();
                    break;
                case Constants.SingleRoundManager.KingOfHearts:
                    round = new KingOfHeartsRound();
                    break;
                case Constants.SingleRoundManager.Piles:
                    round = new PilesRound();
                    break;
                case Constants.SingleRoundManager.Nothing:
                    round = new NothingRound();
                    break;
                case Constants.SingleRoundManager.Everything:
                    round = new EverythingRound();
                    break;
                default:
                    throw new ArgumentException($"Unknown single round: {roundType}");
            }

            var roundList = new List<IRound>()
            {
                round,
            };
            return this.CreateRoundWorkflow(GameTypes.Single, roundList);
        }

        public RoundWorkflow.RoundWorkflow CreateChaosRoundWorkflow()
        {
            var chaosRound = new ChaosRound(this.telemetryService);

            var roundsToMerge = (int)Mathf.Floor(UnityEngine.Random.Range(2.0f, 4.0f));
            for (int i = 0; i < roundsToMerge; i++)
            {
                var round = RoundRegistration.GetRandomRound();
                chaosRound.MergeRound(round);
            }

            var roundList = new List<IRound>()
            {
                chaosRound,
            };
            return this.CreateRoundWorkflow(GameTypes.Chaos, roundList);
        }

        private RoundWorkflow.RoundWorkflow CreateRoundWorkflow(GameTypes gameType, List<IRound> rounds)
        {
            return this.roundWorkflowFactory.Create(gameType, rounds);
        }
    }
}
