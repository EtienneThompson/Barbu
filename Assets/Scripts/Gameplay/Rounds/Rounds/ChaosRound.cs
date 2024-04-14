namespace Barbu.Gameplay.Rounds.Rounds
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Gameplay.Rounds;
    using Barbu.Interfaces.Core;
    using UnityEngine;

    public class ChaosRound : Round
    {
        public override Dictionary<string, int> PointMapping => ComputedPointMapping;
        public override int PointsPerPile => this.ComputedPointsPerPile;
        public override int TotalPoints
        {
            get
            {
                int total = 0;
                foreach (var points in this.PointMapping)
                {
                    total += points.Value;
                }

                total += Constants.NumPilesPerRound * this.ComputedPointsPerPile;
                return total;
            }
        }
        public override string Name
        {
            get
            {
                string nameBuilder = string.Empty;
                foreach (var round in this.mergedRounds)
                {
                    if (!nameBuilder.Contains(round.Name))
                    {
                        nameBuilder += round.Name + "\n";
                    }
                }

                return nameBuilder.Trim();
            }
        }

        private readonly ITelemetryService telemetryService;
        private Dictionary<string, int> ComputedPointMapping;
        private int ComputedPointsPerPile;
        private List<Round> mergedRounds;

        public ChaosRound(RoundContext context)
        : base(context)
        {
            this.telemetryService = TelemetryService.GetInstance();
            this.ComputedPointMapping = new Dictionary<string, int>();
            this.ComputedPointsPerPile = 0;
            this.mergedRounds = new List<Round>();
        }

        public ChaosRound(RoundContext context, Round next)
        : base(context, next)
        {
            this.ComputedPointMapping = new Dictionary<string, int>();
            this.ComputedPointsPerPile = 0;
            this.mergedRounds = new List<Round>();
        }

        /// <summary>
        /// Only end chaos rounds when all piles have been played.
        /// </summary>
        public override bool IsRoundOver(int round, Dictionary<string, int[]> playerPoints, int pilesPlayed)
        {
            return pilesPlayed == Constants.NumPilesPerRound;
        }

        public void MergeRound(Round round)
        {
            this.telemetryService.LogInfo("Merging round " + round.Name);
            this.mergedRounds.Add(round);
            foreach (var points in round.PointMapping)
            {
                if (!this.PointMapping.ContainsKey(points.Key))
                {
                    this.PointMapping[points.Key] = 0;
                }
                this.PointMapping[points.Key] += points.Value;
            }

            this.ComputedPointsPerPile += round.PointsPerPile;
        }
    }
}
