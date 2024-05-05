namespace Barbu.Gameplay.Rounds
{
    using System;
    using System.Collections.Generic;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Interfaces.Rounds;
    using UnityEngine;

    public class RoundFactory
    {
        public RoundFactory()
        {
        }

        public static RoundWorkflow CreateTraditionalRoundWorkflow()
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

            var workflow = new RoundWorkflow(Statistics.GameTypes.Traditional, roundList);
            return workflow;
        }

        public static RoundWorkflow CreateSingleRoundWorkflow(string roundType)
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
            var workflow = new RoundWorkflow(Statistics.GameTypes.Single, roundList);
            return workflow;
        }

        public static RoundWorkflow CreateChaosRoundWorkflow()
        {
            var chaosRound = new ChaosRound();

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
            var workflow = new RoundWorkflow(Statistics.GameTypes.Chaos, roundList);
            return workflow;
        }
    }
}
