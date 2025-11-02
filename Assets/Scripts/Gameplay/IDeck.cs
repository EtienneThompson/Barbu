namespace Barbu
{
    using System;
    using System.Collections.Generic;
    using Barbu.Gameplay;

    public interface IDeck
    {
        int CardsInDeck { get; }

        /// <summary>
        /// Draws one card from the top of the deck.
        /// </summary>
        /// <param name="playerId">The player that drew the card.</param>
        /// <returns>The drawn card.</returns>
        /// <exception cref="ArgumentNullException">No playerId is provided.</exception>
        /// <exception cref="InvalidOperationException"?>If there are no cards remaining in the deck.</exception>
        Card DrawCard(string playerId);

        /// <summary>
        /// Attempts to draw a card from the top of the deck.
        /// </summary>
        /// <param name="playerId">The player that drew the card.</param>
        /// <param name="card">The card that was drawn from the deck</param>
        /// <returns>Whether there was a card to draw from the deck.</returns>
        bool TryDrawCard(string playerId, out Card card);

        /// <summary>
        /// Draws cards from the top of the deck.
        /// </summary>
        /// <param name="playerId">The player that drew cards.</param>
        /// <param name="numCards">The number of cards to draw.</param>
        /// <returns>A list of the drawn cards.</returns>
        /// <exception cref="ArgumentOutOfRangeException">numCards is negative.</exception>
        /// <exception cref="ArgumentNullException">No playerId is provided</exception>
        /// <exception cref="InvalidOperationException">There are not enough cards in the deck to draw numCards.</exception>
        List<Card> DrawCards(string playerId, int numCards);

        /// <summary>
        /// Attempts to draw cards from the top of the deck.
        /// </summary>
        /// <param name="playerId">The player that drew cards.</param>
        /// <param name="numCards">The number of cards to draw.</param>
        /// <param name="cards">The list of drawn cards.</param>
        /// <returns>Whether there were enough cards to draw from the deck.</returns>
        bool TryDrawCards(string playerId, int numCards, out List<Card> cards);

        /// <summary>
        /// Simulates a riffle shuffle on the cards in the deck. It does this by
        /// splitting the deck roughly in half based on the randomnessFactor, and
        /// then merges the two halfs together by roughly taking every other card,
        /// again based on the randomnessFactor.
        /// 
        /// A randomnessFactor of 0.0f represents no randomness and would be a
        /// perfect riffle shuffle. A randomness factor of 1.0f would be the would
        /// reprsent a bad riffle shuffle where likely most of the cards would end
        /// up relatively unshuffled.
        /// </summary>
        /// <param name="randomnessFactor">A decimal value to determine random effects. Range from 0.0f to 1.0f</param>
        void RiffleShuffle(float randomnessFactor = 0.1f);

        void OverhandShuffle(float randomnessFactor = 0.1f);

        void WashShuffle(float randomnessFactor = 0.1f);

        void ShuffleComplete(float randomnessFactor = 0.1f);

        void CutDeck(float randomnessFactor = 0.1f);

        void ResetDeck();
    }
}
