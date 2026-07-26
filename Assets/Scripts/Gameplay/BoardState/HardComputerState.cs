namespace Barbu.Gameplay.BoardState
{
    using System;
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;

    public class HardComputerState : GameState
    {
        public HardComputerState(
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IRound round,
            string id,
            Hand hand)
        : base(stateMachine, telemetryService, round, hand, id)
        {
        }

        public override void Start()
        {
            if (!this.stateMachine.IsCardPlayable())
            {
                throw new Exception("Computer can't make a move right now.");
            }

            this.stateMachine.SetCardPlayable(false);
            var isPositiveRound = this.round.IsRoundPositive();
            var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
            var playableCards = cardsInSuit.Count > 0 ? cardsInSuit : this.hand.GetAvailableCards();

            this.telemetryService.LogInfo("Player " + this.PlayerId + " cards");

            if (cardsInSuit.Count == 0)
            {
                this.telemetryService.LogInfo("Computer has no cards in suit");
                var pointEarningCards = this.hand.GetCardsWithPoints(this.round);

                if (isPositiveRound)
                {
                    // Being out of the suit means this pile goes to somebody else,
                    // and they score whatever gets thrown into it. Points are worth
                    // having in a positive round, so hold onto the point earning
                    // cards and throw away the lowest card that scores nothing -
                    // keeping the high cards to win piles with later.
                    var discardableCards = this.hand.GetAvailableCards();
                    discardableCards.RemoveAll(card => this.round.IsPointEarningCard(card.GetName()));

                    if (discardableCards.Count > 0)
                    {
                        this.telemetryService.LogInfo("Computer is holding its point earning cards, playing lowest card without points.");
                        var lowestCard = this.hand.GetLowestCard(discardableCards);
                        this.telemetryService.LogInfo("Computer playing " + lowestCard.GetName());
                        lowestCard.PlayCard();
                        return;
                    }

                    // Nothing but point cards left, so one has to be given up.
                    this.telemetryService.LogInfo("Computer only has point earning cards left, giving up the cheapest one.");
                    var cheapestCard = this.GetMostValuablePointCard(pointEarningCards, preferLowestRank: true);
                    this.telemetryService.LogInfo("Computer playing " + cheapestCard.GetName());
                    cheapestCard.PlayCard();
                    return;
                }

                // Play the highest point-earning card.
                if (pointEarningCards.Count > 0)
                {
                    this.telemetryService.LogInfo("Computer has point earning cards, picking max value.");
                    var maxCard = this.GetMostValuablePointCard(pointEarningCards, preferLowestRank: false);
                    this.telemetryService.LogInfo("Computer playing " + maxCard.GetName());
                    maxCard.PlayCard();
                    return;
                }
                else
                {
                    this.telemetryService.LogInfo("Computer has no point earning cards, playing highest card.");
                    // If no cards earn points in the hand, then just play any card.
                    var highestCard = this.hand.GetHighestCard();
                    this.telemetryService.LogInfo("Computer playing " + highestCard.GetName());
                    highestCard.PlayCard();
                    return;
                }
            }
            else
            {
                this.telemetryService.LogInfo("Computer has cards in starting suit");
                if (playableCards.Count == 1)
                {
                    this.telemetryService.LogInfo("Computer has only one card in starting suit, playing it");
                    this.telemetryService.LogInfo("Computer playing " + playableCards[0].GetName());
                    // If there's only one available card to play, play it.
                    playableCards[0].PlayCard();
                    return;
                }
                else
                {
                    this.telemetryService.LogInfo("Computer playing based on turn order");
                    this.telemetryService.LogInfo("Computer is player " + this.stateMachine.NumCardsPlayed());
                    if (this.stateMachine.NumCardsPlayed() == 0)
                    {
                        this.telemetryService.LogInfo("Computer playing first, using lowest card");
                        this.telemetryService.LogInfo("Number of cards to choose from: " + playableCards.Count);
                        var lowestCard = isPositiveRound ? this.hand.GetHighestCard(playableCards) : this.hand.GetLowestCard(playableCards);
                        this.telemetryService.LogInfo("Computer playing " + lowestCard.GetName());
                        lowestCard.PlayCard();
                        return;
                    }
                    else if (this.stateMachine.NumCardsPlayed() == 1 || this.stateMachine.NumCardsPlayed() == 2)
                    {
                        this.telemetryService.LogInfo("Computer playing 2nd or 3rd");
                        var bestCard = isPositiveRound ?
                            this.hand.GetCardAboveRank(playableCards, this.stateMachine.GetHighestRankedCard()) :
                            this.hand.GetCardBelowRank(playableCards, this.stateMachine.GetHighestRankedCard(), useLowestFallback: true);
                        this.telemetryService.LogInfo("Computer playing " + bestCard.GetName());
                        bestCard.PlayCard();
                        return;
                    }
                    else
                    {
                        this.telemetryService.LogInfo("Computer going last");
                        var bestCard = isPositiveRound ?
                            this.hand.GetCardAboveRank(playableCards, this.stateMachine.GetHighestRankedCard()) :
                            this.hand.GetCardBelowRank(playableCards, this.stateMachine.GetHighestRankedCard(), useLowestFallback: false);
                        this.telemetryService.LogInfo("Computer playing " + bestCard.GetName());
                        bestCard.PlayCard();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Picks the point earning card with the highest value: the biggest penalty
        /// to hand off in a round that scores against the computer, or the cheapest
        /// card to give up in a positive round where nothing else is left to play.
        /// Ties are broken by rank - highest when dumping penalties, lowest in a
        /// positive round where the high cards are what win piles later.
        /// </summary>
        private Card GetMostValuablePointCard(List<Card> pointEarningCards, bool preferLowestRank)
        {
            var maxPoints = int.MinValue;
            Card maxCard = null;
            foreach (var card in pointEarningCards)
            {
                var cardPointValue = this.round.GetCardPointValue(card.GetName());
                var isBetterRank = maxCard == null ||
                    (preferLowestRank ? card.rank < maxCard.rank : card.rank > maxCard.rank);
                if (cardPointValue >= maxPoints && isBetterRank)
                {
                    maxPoints = cardPointValue;
                    maxCard = card;
                }
            }

            return maxCard;
        }
    }
}