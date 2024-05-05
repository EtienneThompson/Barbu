using Barbu.Gameplay.Rounds.Rounds;
using Barbu.Interfaces.Rounds;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barbu.Gameplay.Rounds
{
    public class RoundFactory
    {
        public RoundFactory()
        {
        }

        public static RoundWorkflow CreateTraditionalRoundWorkflow()
        {
            var heartsRound = new HeartsRound(null);
            var queensRound = new QueensRound(null);
            var kohRound = new KingOfHeartsRound(null);
            var pilesRound = new PilesRound(null);
            var nothingRound = new NothingRound(null);
            var everythingRound = new EverythingRound(null);
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
                    round = new HeartsRound(null);
                    break;
                case Constants.SingleRoundManager.Queens:
                    round = new QueensRound(null);
                    break;
                case Constants.SingleRoundManager.KingOfHearts:
                    round = new KingOfHeartsRound(null);
                    break;
                case Constants.SingleRoundManager.Piles:
                    round = new PilesRound(null);
                    break;
                case Constants.SingleRoundManager.Nothing:
                    round = new NothingRound(null);
                    break;
                case Constants.SingleRoundManager.Everything:
                    round = new EverythingRound(null);
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
            var chaosRound = new ChaosRound(null);

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
