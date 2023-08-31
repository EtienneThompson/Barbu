using System;
using System.Collections.Generic;
using System.Linq;
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
        var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
        var playableCards = cardsInSuit.Count > 0 ? cardsInSuit : this.hand.GetAvailableCards();

        List<CardPlayability> cardPlayability = new List<CardPlayability>();
        foreach (var card in playableCards)
        {
            decimal ranking = 0.0M;
            // If we can play an off suit card, rank all the higher numbers higher.
            if (!card.suit.Equals(this.stateMachine.GetStartingSuit(), StringComparison.OrdinalIgnoreCase))
            {
                ranking += (decimal)card.rank;
                if (this.context.IsPointEarningCard(card.GetName()))
                {
                    // Multiply point earning cards by 1.5, incentivizing aces and kings of non-point suits
                    // over say 4s or 6s or point earning suits.
                    ranking *= 1.5M;
                }
            }
            else
            {
                // Increase the ranking of each card lower than the current highest opposite to it's rank.
                if (card.rank < this.stateMachine.GetHighestRankedCard())
                {
                    ranking += 14 - card.rank;
                }
            }

            Debug.Log("Card: " + card.GetName());
            Debug.Log("Ranking: " + ranking);

            cardPlayability.Add(new CardPlayability
            {
                Card = card,
                Ranking = ranking,
            });
        }

        // Sort the ranking.
        cardPlayability.OrderBy(p => p.Ranking);
        cardPlayability[0].Card.PlayCard();
    }
}
