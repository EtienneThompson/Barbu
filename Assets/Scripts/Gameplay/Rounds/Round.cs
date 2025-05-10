namespace Barbu.Gameplay.Rounds
{
    using System;
    using System.Collections.Generic;

    public class Round : IRound
    {
        public virtual Dictionary<string, int> PointMapping => new Dictionary<string, int>();
        public virtual int PointsPerPile => 0;
        public virtual int TotalPoints => 0;
        public virtual string Name => throw new NotImplementedException("Name needs to be overridden");

        public Round()
        {
        }

        public int CalculatePointsInPile(Pile pile)
        {
            int points = 0;
            foreach (var card in pile.GetCards())
            {
                if (this.PointMapping.TryGetValue(card.GetName(), out var cardValue))
                {
                    points += cardValue;
                }
            }

            return points + this.PointsPerPile;
        }

        public int CalculatePointsInAllPiles(List<Pile> piles)
        {
            int points = 0;
            foreach (var pile in piles)
            {
                points += this.CalculatePointsInPile(pile);
            }

            return points;
        }

        public virtual bool IsRoundOver(int round, Dictionary<string, int[]> playerPoints, int playedPiles)
        {
            int points = 0;
            foreach (var key in playerPoints.Keys)
            {
                points += playerPoints[key][round];
            }

            return points == this.TotalPoints;
        }

        public virtual bool IsRoundPositive()
        {
            return this.TotalPoints < 0;
        }

        public bool IsPointEarningCard(string cardName)
        {
            return this.PointMapping.ContainsKey(cardName);
        }

        public int GetCardPointValue(string cardName)
        {
            return this.PointMapping.TryGetValue(cardName, out var value) ? value : 0;
        }
    }
}