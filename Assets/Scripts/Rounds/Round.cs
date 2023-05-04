using System;
using System.Collections;
using System.Collections.Generic;

public class Round : IRound
{
    protected virtual Dictionary<string, int> PointMapping => new Dictionary<string, int>();
    protected virtual int PointsPerPile => 0;
    protected virtual int TotalPoints => 0;
    protected RoundContext context;
    protected Round nextState;

    public Round(RoundContext context)
    {
        this.context = context;
    }

    public Round(RoundContext context, Round next)
    : this(context)
    {
        this.nextState = next;
    }

    public void GoNext()
    {
        if (this.nextState == null)
        {
            throw new Exception("No next state set.");
        }

        this.context.SetState(this.nextState);
    }

    public void SetNextState(Round next)
    {
        this.nextState = next;
    }

    public int CalculatePointsInPile(Card[] pile)
    {
        int points = 0;
        foreach (var card in pile)
        {
            if (this.PointMapping.TryGetValue(card.GetName(), out var cardValue))
            {
                points += cardValue;
            }
        }

        return points + this.PointsPerPile;
    }
    public int CalculatePointsInAllPiles(List<Card[]> piles)
    {
        int points = 0;
        foreach (var pile in piles)
        {
            points += this.CalculatePointsInPile(pile);
        }

        return points;
    }

    public bool IsRoundOver(int round, Dictionary<string, int[]> playerPoints)
    {
        int points = 0;
        foreach (var key in playerPoints.Keys)
        {
            points += playerPoints[key][round];
        }

        return points == this.TotalPoints;
    }
}
