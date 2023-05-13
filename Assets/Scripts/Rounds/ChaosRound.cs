using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosRound : Round
{
    public override Dictionary<string, int> PointMapping => new Dictionary<string, int>();
    public override int PointsPerPile => this.ComputedPointsPerPile;
    public override int TotalPoints => this.ComputedTotalPoints;
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
    private int ComputedPointsPerPile;
    private int ComputedTotalPoints;
    private List<Round> mergedRounds;

    public ChaosRound(RoundContext context)
    : base(context)
    {
        this.ComputedPointsPerPile = 0;
        this.ComputedTotalPoints = 0;
        this.mergedRounds = new List<Round>();
    }

    public ChaosRound(RoundContext context, Round next)
    : base(context, next)
    {
        this.ComputedPointsPerPile = 0;
        this.ComputedTotalPoints = 0;
        this.mergedRounds = new List<Round>();
    }

    public void MergeRound(Round round)
    {
        Debug.Log("Merging round " + round.Name);
        this.mergedRounds.Add(round);
        foreach (var points in round.PointMapping)
        {
            if (!this.PointMapping.ContainsKey(points.Key))
            {
                this.PointMapping[points.Key] = points.Value;
                this.ComputedTotalPoints += points.Value;
            }
        }

        if (round.PointsPerPile > 0 && this.ComputedPointsPerPile == 0)
        {
            this.ComputedPointsPerPile = round.PointsPerPile;
            this.ComputedTotalPoints += 13 * round.PointsPerPile;
        }
    }
}
