namespace Barbu.Gameplay.Rounds
{
    using System;
    using System.Collections.Generic;
    using Barbu.Interfaces.Rounds;

    public class Round : IRound
    {
        public virtual Dictionary<string, int> PointMapping => new Dictionary<string, int>();
        public virtual int PointsPerPile => 0;
        public virtual int TotalPoints => 0;
        public virtual string Name => throw new NotImplementedException("Name needs to be overridden");
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