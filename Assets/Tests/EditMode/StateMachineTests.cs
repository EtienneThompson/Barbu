namespace Barbu.Tests.EditMode
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Gameplay;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class StateMachineTests
    {
        private readonly List<Card> createdCards = new();
        private FakeEventsController eventsController;
        private StateMachine stateMachine;

        [SetUp]
        public void SetUp()
        {
            this.eventsController = new FakeEventsController();
            this.stateMachine = new StateMachine(this.eventsController);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var card in this.createdCards)
            {
                UnityEngine.Object.DestroyImmediate(card.gameObject);
            }

            this.createdCards.Clear();
        }

        [Test]
        public void IncrementAndResetNumCardsPlayed_TrackCount()
        {
            this.stateMachine.IncrementNumCardsPlayed();
            this.stateMachine.IncrementNumCardsPlayed();
            Assert.AreEqual(2, this.stateMachine.NumCardsPlayed());

            this.stateMachine.ResetNumCardsPlayed();
            Assert.AreEqual(0, this.stateMachine.NumCardsPlayed());
        }

        [Test]
        public void SetMenuOpen_True_PausesGameAndFiresPauseEvent()
        {
            this.stateMachine.SetMenuOpen(true);

            Assert.IsTrue(this.stateMachine.IsMenuOpen());
            Assert.IsTrue(this.stateMachine.IsGamePaused());
            CollectionAssert.Contains(this.eventsController.FiredEvents, EventNames.PauseGame);
        }

        [Test]
        public void SetMenuOpen_False_ResumesGameAndFiresResumeEvent()
        {
            this.stateMachine.SetMenuOpen(true);

            this.stateMachine.SetMenuOpen(false);

            Assert.IsFalse(this.stateMachine.IsMenuOpen());
            Assert.IsFalse(this.stateMachine.IsGamePaused());
            CollectionAssert.Contains(this.eventsController.FiredEvents, EventNames.ResumeGame);
        }

        [Test]
        public void SetPlayerMustPlayStartingSuit_NonStartingPlayerHoldingStartingSuit_MustFollow()
        {
            this.stateMachine.SetStartingSuit("Heart");
            var hand = new Hand();
            var card = CardTestUtils.CreateCard("Heart", "05");
            this.createdCards.Add(card);
            hand.AddCard(card);
            for (int i = 1; i < 13; i++)
            {
                var filler = CardTestUtils.CreateCard("Club", "02");
                this.createdCards.Add(filler);
                hand.AddCard(filler);
            }

            this.stateMachine.SetPlayerMustPlayStartingSuit(isStartingPlayer: false, hand);

            Assert.IsTrue(this.stateMachine.MustPlayCardInStartingSuit());
        }

        [Test]
        public void SetPlayerMustPlayStartingSuit_StartingPlayer_NeverMustFollow()
        {
            this.stateMachine.SetStartingSuit("Heart");
            var hand = new Hand();
            for (int i = 0; i < 13; i++)
            {
                var card = CardTestUtils.CreateCard("Heart", "05");
                this.createdCards.Add(card);
                hand.AddCard(card);
            }

            this.stateMachine.SetPlayerMustPlayStartingSuit(isStartingPlayer: true, hand);

            Assert.IsFalse(this.stateMachine.MustPlayCardInStartingSuit());
        }

        [Test]
        public void AutoPlayMode_RoundTrips()
        {
            Assert.IsFalse(this.stateMachine.IsAutoPlayMode());

            this.stateMachine.SetAutoPlayMode(true);

            Assert.IsTrue(this.stateMachine.IsAutoPlayMode());
        }
    }
}
