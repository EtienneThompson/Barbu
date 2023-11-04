using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand
{
    private Card[] cards;
    private int cardsInHand;

    public Hand()
    {
        this.cards = new Card[13];
        this.cardsInHand = 0;
    }

    public void AddCard(Card card)
    {
        this.cards[this.cardsInHand] = card;
        this.cardsInHand++;
    }

    public Card[] GetHand()
    {
        return this.cards;
    }

    public Card GetCardAtPosition(int position)
    {
        if (position < 0 || position > this.cards.Length)
        {
            throw new IndexOutOfRangeException("Must be a valid card in the hand.");
        }

        return this.cards[position];
    }

    public List<Card> CardsInSuit(string suit)
    {
        var suitCards = new List<Card>();
        foreach (var card in this.cards)
        {
            if (card.state != Card.CardState.Played && 
                (string.IsNullOrEmpty(suit) || card.suit.Equals(suit, StringComparison.OrdinalIgnoreCase)))
            {
                suitCards.Add(card);
            }
        }

        return suitCards;
    }

    public List<Card> GetCardsWithPoints(GameStateContext context, bool includePlayed = false)
    {
        var pointCards = new List<Card>();
        foreach (var card in this.cards)
        {
            if (!includePlayed && card.state != Card.CardState.Waiting)
            {
                continue;
            }

            if (context.IsPointEarningCard(card.GetName()))
            {
                pointCards.Add(card);
            }
        }

        return pointCards;
    }

    public List<Card> GetAvailableCards()
    {
        var availableCards = new List<Card>();
        foreach (var card in this.cards)
        {
            if (card.state != Card.CardState.Played)
            {
                availableCards.Add(card);
            }
        }

        return availableCards;
    }

    public Card GetHighestCard(bool includePlayed = false)
    {
        // Start with null so that if all cards are played and includePlayed
        // is false, we don't return a card.
        Card maxRank = null;
        foreach (var card in this.cards)
        {
            if (!includePlayed && card.state != Card.CardState.Waiting)
            {
                continue;
            }

            if (maxRank == null || card.rank > maxRank.rank)
            {
                maxRank = card;
            }
        }

        return maxRank;
    }

    public Card GetHighestCard(List<Card> subset)
    {
        Card maxRank = null;
        foreach (var card in subset)
        {
            if (maxRank == null || card.rank > maxRank.rank)
            {
                maxRank = card;
            }
        }

        return maxRank;
    }

    public Card GetLowestCard(bool includePlayed = false)
    {
        Card minRank = null;
        foreach (var card in this.cards)
        {
            if (!includePlayed && card.state != Card.CardState.Waiting)
            {
                continue;
            }

            if (minRank == null || card.rank < minRank.rank)
            {
                minRank = card;
            }
        }

        return minRank;
    }

    public Card GetLowestCard(List<Card> subset)
    {
        Card minRank = null;
        foreach (var card in subset)
        {
            if (minRank == null || card.rank < minRank.rank)
            {
                minRank = card;
            }
        }

        return minRank;
    }

    public Card GetCardBelowRank(List<Card> subset, int rank, bool useLowestFallback = false)
    {
        Card maxRank = null;
        foreach (var card in subset)
        {
            if (card.rank < rank && (maxRank == null || card.rank > maxRank.rank))
            {
                maxRank = card;
            }
        }

        if (maxRank == null && useLowestFallback)
        {
            return this.GetLowestCard(subset);
        }
        else if (maxRank == null && !useLowestFallback)
        {
            return this.GetHighestCard(subset);
        }

        return maxRank;
    }

    public Card GetCardAboveRank(List<Card> subset, int rank)
    {
        var maxCard = this.GetHighestCard(subset);
        if (maxCard.rank >= rank)
        {
            return maxCard;
        }
        else
        {
            return this.GetLowestCard(subset);
        }
    }

    public void SortHand(Settings.SortingOptions option)
    {
        this.cards = this.cards.OrderBy(c => c.GetSortingRank(option)).ToArray();
    }
}
