namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Gameplay;

    /// <summary>
    /// Hand always allocates a fixed 13-slot array, so a partially filled hand
    /// leaves null slots that most Hand/GameState methods throw on - always
    /// build a full 13-card hand.
    /// </summary>
    public static class HandTestUtils
    {
        public static Hand BuildHand(List<Card> trackedCards, IStateMachine stateMachine, params (string suit, string rank)[] specs)
        {
            var hand = new Hand();
            foreach (var (suit, rank) in specs)
            {
                var card = CardTestUtils.CreateCard(suit, rank, stateMachine: stateMachine);
                trackedCards.Add(card);
                hand.AddCard(card);
            }

            return hand;
        }

        public static Hand BuildUniformHand(List<Card> trackedCards, IStateMachine stateMachine, string suit, string rank)
        {
            var specs = new (string suit, string rank)[13];
            for (int i = 0; i < specs.Length; i++)
            {
                specs[i] = (suit, rank);
            }

            return BuildHand(trackedCards, stateMachine, specs);
        }
    }
}
