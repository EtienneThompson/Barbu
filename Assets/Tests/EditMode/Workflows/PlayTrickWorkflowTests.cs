namespace Barbu.Tests.EditMode.Workflows
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;
    using PlayTrickWorkflow = Barbu.Core.Workflows.PlayTrickWorkflow.PlayTrickWorkflow;

    // Only the constructor is covered here - see PlayCardStepTests and
    // HandleCardPlayedStepTests for the steps themselves. ResolveTrickStep is
    // not covered: it calls GameObject.Find(Constants.GameObjects.InGamePoints)
    // for a real InGamePointsController, which needs a live scene.
    public class PlayTrickWorkflowTests
    {
        private FakeEventsController eventsController;
        private FakeStateMachine stateMachine;
        private Dictionary<string, int[]> playerPoints;
        private Hand[] playerHands;

        [SetUp]
        public void SetUp()
        {
            this.eventsController = new FakeEventsController();
            this.stateMachine = new FakeStateMachine();
            this.playerPoints = new Dictionary<string, int[]>
            {
                [Constants.PlayerIds.Player1] = new int[1],
                [Constants.PlayerIds.Player2] = new int[1],
                [Constants.PlayerIds.Player3] = new int[1],
                [Constants.PlayerIds.Player4] = new int[1],
            };
            this.playerHands = new[] { new Hand(), new Hand(), new Hand(), new Hand() };
        }

        [Test]
        public void Constructor_AutoPlayMode_RequestsComputerStateForAllFourPlayers()
        {
            this.stateMachine.SetAutoPlayMode(true);
            var factory = new FakeComputerStateFactory();

            _ = new PlayTrickWorkflow(
                this.eventsController, this.stateMachine, new FakeTelemetryService(), factory,
                new HeartsRound(), this.playerPoints, this.playerHands, startingPlayer: 0, roundNumber: 0);

            CollectionAssert.AreEquivalent(
                new[] { Constants.PlayerIds.Player1, Constants.PlayerIds.Player2, Constants.PlayerIds.Player3, Constants.PlayerIds.Player4 },
                factory.RequestedPlayerIds);
        }

        [Test]
        public void Constructor_NotAutoPlayMode_DoesNotRequestComputerStateForPlayerOne()
        {
            this.stateMachine.SetAutoPlayMode(false);
            var factory = new FakeComputerStateFactory();

            _ = new PlayTrickWorkflow(
                this.eventsController, this.stateMachine, new FakeTelemetryService(), factory,
                new HeartsRound(), this.playerPoints, this.playerHands, startingPlayer: 0, roundNumber: 0);

            CollectionAssert.DoesNotContain(factory.RequestedPlayerIds, Constants.PlayerIds.Player1);
            CollectionAssert.AreEquivalent(
                new[] { Constants.PlayerIds.Player2, Constants.PlayerIds.Player3, Constants.PlayerIds.Player4 },
                factory.RequestedPlayerIds);
        }

        [Test]
        public void Constructor_ClearsStartingSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            var factory = new FakeComputerStateFactory();

            _ = new PlayTrickWorkflow(
                this.eventsController, this.stateMachine, new FakeTelemetryService(), factory,
                new HeartsRound(), this.playerPoints, this.playerHands, startingPlayer: 0, roundNumber: 0);

            Assert.AreEqual(string.Empty, this.stateMachine.GetStartingSuit());
        }
    }
}
