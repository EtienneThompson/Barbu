namespace Barbu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Barbu.Gameplay;
    using UnityEngine;

    public class Deck : IDeck
    {
        private const int TotalCards = 52;

        private static string[] cardSuits = new string[] { "Club", "Diamond", "Spade", "Heart" };
        // Ranks must match the resources used, 01 is A and 11, 12, and 13 are J, Q, and K.
        private static string[] cardRanks = new string[] { "01", "02", "03", "04", "05", "06", "07",
                                                 "08", "09", "10", "11", "12", "13" };

        private readonly ICardFactory cardFactory;
        private readonly System.Random random;
        private List<string> cards;
        private int cardsDrawn;

        public Deck(ICardFactory cardFactory)
        {
            this.cardFactory = cardFactory;
            this.random = new System.Random();

            this.ResetDeck();
        }

        public int CardsInDeck => this.cards.Count;

        public Card DrawCard(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                throw new ArgumentNullException(nameof(playerId));

            if (this.cardsDrawn >= TotalCards)
                throw new InvalidOperationException("There are no more cards to draw in the deck");

            var cardToDraw = this.cards.First();
            this.cards.Remove(cardToDraw);
            this.cardsDrawn++;
            var suit = cardToDraw.Substring(0, cardToDraw.Length - 2);
            var rank = cardToDraw.Substring(cardToDraw.Length - 2);
            var card = this.cardFactory.CreateCard(suit, rank, playerId);
            return card;
        }

        public bool TryDrawCard(string playerId, out Card card)
        {
            try
            {
                card = this.DrawCard(playerId);
                return true;
            }
            catch (Exception)
            {
                card = null;
                return false;
            }
        }

        public List<Card> DrawCards(string playerId, int numCards)
        {
            if (numCards < 0)
                throw new ArgumentOutOfRangeException(nameof(numCards));

            List<Card> cards = new();
            for (int i = 0; i < numCards; i++)
            {
                var card = this.DrawCard(playerId);
                cards.Add(card);
            }

            return cards;
        }

        public bool TryDrawCards(string playerId, int numCards, out List<Card> cards)
        {
            try
            {
                cards = this.DrawCards(playerId, numCards);
                return true;
            }
            catch (Exception)
            {
                cards = null;
                return false;
            }
        }

        public void OverhandShuffle(float randomnessFactor = 0.01F)
        {
            // The randomness factor makes splitting the deck in half imperfect.
            var splitGap = (int)Math.Round(10 * randomnessFactor, 0);
            var minValue = (this.cards.Count / 2) - splitGap;
            var maxValue = (this.cards.Count / 2) + splitGap;
            var deckSplit = this.random.Next(minValue, maxValue);

            var minDeckPosition = (this.cards.Count / 4) - splitGap;
            var maxDeckPosition = (this.cards.Count / 4) + splitGap;
            var startingDeckPosition = this.random.Next(minDeckPosition, maxDeckPosition);

            // Split the deck into 3 sections.
            var startCards = this.cards.Take(startingDeckPosition).ToList();
            this.cards.RemoveRange(0, startingDeckPosition);
            var shuffleCards = this.cards.Take(deckSplit).ToList();
            this.cards.RemoveRange(0, deckSplit);
            var remainingCards = this.cards;

            var newCards = startCards.Concat(remainingCards).ToList();

            int numSplits = randomnessFactor == 0.0f ? 5 : this.random.Next(4, 6);
            for (int i = 0; i < numSplits; i++)
            {
                // On the last split, just take the remaining cards to combine.
                if (i == numSplits - 1)
                {
                    newCards = shuffleCards.Concat(newCards).ToList();
                }
                else
                {
                    var defaultSplitAmount = shuffleCards.Count / numSplits;
                    int splitAmounts = randomnessFactor == 0.0f ? defaultSplitAmount : this.random.Next(defaultSplitAmount - 1, defaultSplitAmount + 1);
                    var splitCards = shuffleCards.Take(splitAmounts);
                    shuffleCards.RemoveRange(0, splitAmounts);
                    newCards = splitCards.Concat(newCards).ToList();
                }
            }
        }

        public void RiffleShuffle(float randomnessFactor = 0.1F)
        {
            // The randomness factor makes splitting the deck in half imperfect.
            var splitGap = (int)Math.Round(10 * randomnessFactor, 0);
            var minValue = (this.cards.Count / 2) - splitGap;
            var maxValue = (this.cards.Count / 2) + splitGap;
            var deckSplit = this.random.Next(minValue, maxValue);

            // Split the cards based on chosen split.
            var firstHalf = this.cards.Take(deckSplit).ToList();
            var secondHalf = this.cards.TakeLast(this.cards.Count - deckSplit).ToList();

            var shuffledCards = new List<string>();
            while (firstHalf.Count > 0 || secondHalf.Count > 0)
            {
                if (firstHalf.Count > 0 && (secondHalf.Count == 0 || random.Next(2) == 0))
                {
                    shuffledCards.Add(firstHalf[0]);
                    firstHalf.RemoveAt(0);
                }
                else if (secondHalf.Count > 0)
                {
                    shuffledCards.Add(secondHalf[0]);
                    secondHalf.RemoveAt(0);
                }
            }

            this.cards = shuffledCards;
        }

        public void WashShuffle(float randomnessFactor = 0.1F)
        {
            // TODO: write a better wash algorithm. For now, just randomly swap card positions.
            for (int i = 0; i < 5; i++)
            {
                int n = this.cards.Count;
                while (n > 1)
                {
                    int k = (int)Math.Floor(this.random.NextDouble() * (n--));
                    var temp = this.cards[n];
                    this.cards[n] = this.cards[k];
                    this.cards[k] = temp;
                }
            }
        }

        public void ShuffleComplete(float randomnessFactor = 0.1F)
        {
            this.WashShuffle(randomnessFactor);
            //this.RiffleShuffle(randomnessFactor);
            //this.RiffleShuffle(randomnessFactor);
            //this.OverhandShuffle(randomnessFactor);
            //this.RiffleShuffle(randomnessFactor);
            //this.RiffleShuffle(randomnessFactor);
            //this.CutDeck(randomnessFactor);
        }

        public void CutDeck(float randomnessFactor = 0.1f)
        {
            // The randomness factor makes splitting the deck in half imperfect.
            var splitGap = (int)Math.Round(10 * randomnessFactor, 0);
            var minValue = (this.cards.Count / 2) - splitGap;
            var maxValue = (this.cards.Count / 2) + splitGap;
            var deckSplit = this.random.Next(minValue, maxValue);

            var firstHalf = this.cards.Take(deckSplit);
            var secondHalf = this.cards.TakeLast(this.cards.Count - deckSplit);

            this.cards = secondHalf.Concat(firstHalf).ToList();
        }

        public void ResetDeck()
        {
            this.cardsDrawn = 0;
            this.cards = new List<string>();
            for (int i = 0; i < cardSuits.Length; i++)
            {
                for (int j = 0; j < cardRanks.Length; j++)
                {
                    var card = cardSuits[i] + cardRanks[j];
                    this.cards.Add(card);
                }
            }
        }
    }
}
