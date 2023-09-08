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

        Debug.Log("Player " + this.PlayerId + " cards");
        List<CardPlayability> cardPlayability = new List<CardPlayability>();
        foreach (var card in playableCards)
        {
            decimal ranking = 0.0M;
            if (this.stateMachine.NumCardsPlayed() == 0)
            {
                // If the computer is playing first, rank card opposite to their rank.
                ranking += 14 - card.rank;
            }
            else
            {
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
                    // Increase the ranking of each card lower than the current highest in proportion to its rank.
                    if (card.rank < this.stateMachine.GetHighestRankedCard())
                    {
                        ranking += card.rank;
                    }
                    else
                    {
                        // This ranking should peak near the middle cards, and fall off towards the ends.
                        if (card.rank < 6)
                        {
                            ranking += 0.0M;
                        }
                        else
                        {
                            ranking += (decimal)(card.rank * -0.25M + 3.5M);
                        }
                    }

                    // Special logic if they're the last player, increase the ranking of the higher cards.
                    if (this.stateMachine.NumCardsPlayed() == 3)
                    {
                        var multiplier = 1;
                        // Derank higher cards when it is a point earning card.
                        if (this.context.IsPointEarningCard(card.GetName()))
                        {
                            multiplier = -1;
                        };

                        ranking += multiplier * card.rank * 0.07M;
                    }

                    // Special logic if they're the first player, rank the lower cards higher.
                    if (this.stateMachine.NumCardsPlayed() == 0)
                    {
                        ranking += 1 / card.rank;
                    }
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
        Debug.Log("");

        // Sort the ranking.
        cardPlayability = cardPlayability.OrderByDescending(p => p.Ranking).ToList();
        Debug.Log("Playing card: " + cardPlayability[0].Card.GetName());
        Debug.Log("Played ranking: " + cardPlayability[0].Ranking);
        cardPlayability[0].Card.PlayCard();
    }
}
