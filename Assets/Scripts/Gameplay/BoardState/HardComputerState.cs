using System;
using UnityEngine;

public class HardComputerState : GameState
{
    public HardComputerState(GameStateContext context, string id, Hand hand)
    : base(context, hand, id)
    {
    }

    public HardComputerState(GameStateContext context, GameState next, string id, Hand hand)
    : base(context, next, hand, id)
    {
    }

    public override void Start()
    {
        if (!this.stateMachine.IsCardPlayable())
        {
            throw new Exception("Computer can't make a move right now.");
        }

        this.stateMachine.SetCardPlayable(false);
        var isPositiveRound = this.context.IsCurrentRoundPositive();
        var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
        var playableCards = cardsInSuit.Count > 0 ? cardsInSuit : this.hand.GetAvailableCards();

        Debug.Log("Player " + this.PlayerId + " cards");

        if (cardsInSuit.Count == 0)
        {
            Debug.Log("Computer has no cards in suit");
            // Play the highest point-earning card.
            var pointEarningCards = this.hand.GetCardsWithPoints(this.context);

            if (pointEarningCards.Count > 0)
            {
                Debug.Log("Computer has point earning cards, picking max value.");
                var maxPoints = isPositiveRound ? int.MaxValue : int.MinValue;
                Card maxCard = null;
                foreach (var card in pointEarningCards)
                {
                    var cardPointValue = this.context.GetCardPointValue(card.GetName());
                    if (!isPositiveRound && cardPointValue >= maxPoints && (maxCard == null || card.rank > maxCard.rank))
                    {
                        maxPoints = cardPointValue;
                        maxCard = card;
                    } else if (isPositiveRound && cardPointValue <= maxPoints && (maxCard == null || card.rank < maxCard.rank))
                    {
                        maxPoints = cardPointValue;
                        maxCard = card;
                    }
                }

                Debug.Log("Computer playing " + maxCard.GetName());
                maxCard.PlayCard();
                return;
            }
            else
            {
                Debug.Log("Computer has no point earning cards, playing highest card.");
                // If no cards earn points in the hand, then just play any card.
                var highestCard = isPositiveRound ? this.hand.GetLowestCard() : this.hand.GetHighestCard();
                Debug.Log("Computer playing " + highestCard.GetName());
                highestCard.PlayCard();
                return;
            }
        }
        else
        {
            Debug.Log("Computer has cards in starting suit");
            if (playableCards.Count == 1)
            {
                Debug.Log("Computer has only one card in starting suit, playing it");
                Debug.Log("Computer playing " + playableCards[0].GetName());
                // If there's only one available card to play, play it.
                playableCards[0].PlayCard();
                return;
            }
            else
            {
                Debug.Log("Computer playing based on turn order");
                Debug.Log("Computer is player " + this.stateMachine.NumCardsPlayed());
                if (this.stateMachine.NumCardsPlayed() == 0)
                {
                    Debug.Log("Computer playing first, using lowest card");
                    Debug.Log("Number of cards to choose from: " + playableCards.Count);
                    var lowestCard = isPositiveRound ? this.hand.GetHighestCard(playableCards) : this.hand.GetLowestCard(playableCards);
                    Debug.Log("Computer playing " + lowestCard.GetName());
                    lowestCard.PlayCard();
                    return;
                }
                else if (this.stateMachine.NumCardsPlayed() == 1 || this.stateMachine.NumCardsPlayed() == 2)
                {
                    Debug.Log("Computer playing 2nd or 3rd");
                    var bestCard = isPositiveRound ?
                        this.hand.GetCardAboveRank(playableCards, this.stateMachine.GetHighestRankedCard()) :
                        this.hand.GetCardBelowRank(playableCards, this.stateMachine.GetHighestRankedCard(), useLowestFallback: true);
                    Debug.Log("Computer playing " + bestCard.GetName());
                    bestCard.PlayCard();
                    return;
                }
                else
                {
                    Debug.Log("Computer going last");
                    var bestCard = isPositiveRound ?
                        this.hand.GetCardAboveRank(playableCards, this.stateMachine.GetHighestRankedCard()) :
                        this.hand.GetCardBelowRank(playableCards, this.stateMachine.GetHighestRankedCard(), useLowestFallback: false);
                    Debug.Log("Computer playing " + bestCard.GetName());
                    bestCard.PlayCard();
                    return;
                }
            }
        }
    }
}
