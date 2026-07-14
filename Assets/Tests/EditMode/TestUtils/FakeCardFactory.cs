namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.Gameplay;

    /// <summary>
    /// ICardFactory that builds real (but scene-less) Card components instead of
    /// instantiating the BlankPlayingCard prefab through Zenject. Tracks every
    /// Card it creates so tests can destroy the backing GameObjects in TearDown.
    /// </summary>
    public class FakeCardFactory : ICardFactory
    {
        public List<Card> CreatedCards { get; } = new();

        public Card CreateCard(string suit, string rank, string playerId)
        {
            var card = CardTestUtils.CreateCard(suit, rank, playerId);
            this.CreatedCards.Add(card);
            return card;
        }
    }
}
