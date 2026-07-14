namespace Barbu.Tests.EditMode.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class PlayCardStepTests
    {
        private FakeStateMachine stateMachine;
        private FakeWorkflow workflow;
        private PlayCardStep step;
        private RecordingGameState[] gameStates;

        [SetUp]
        public void SetUp()
        {
            this.stateMachine = new FakeStateMachine();
            this.workflow = new FakeWorkflow();
            this.step = new PlayCardStep();
            this.step.Initialize(this.workflow, new FakeEventsController(), this.stateMachine, new FakeTelemetryService());
            this.gameStates = new[]
            {
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player1, new Hand()),
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player2, new Hand()),
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player3, new Hand()),
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player4, new Hand()),
            };
        }

        private StepArguments<PlayTrickArguments> BuildArgs(int currentGameStateIndex) =>
            new StepArguments<PlayTrickArguments>
            {
                Data = new PlayTrickArguments
                {
                    gameStates = this.gameStates,
                    currentGameStateIndex = currentGameStateIndex,
                },
            };

        [Test]
        public async Task InvokeAsync_SetsCardPlayableTrue()
        {
            this.stateMachine.SetCardPlayable(false);

            await this.step.InvokeAsync(this.BuildArgs(currentGameStateIndex: 0));

            Assert.IsTrue(this.stateMachine.IsCardPlayable());
        }

        [Test]
        public async Task InvokeAsync_StartsTheCurrentPlayersGameState()
        {
            await this.step.InvokeAsync(this.BuildArgs(currentGameStateIndex: 2));

            Assert.AreEqual(1, this.gameStates[2].StartCallCount);
            Assert.AreEqual(0, this.gameStates[0].StartCallCount);
            Assert.AreEqual(0, this.gameStates[1].StartCallCount);
            Assert.AreEqual(0, this.gameStates[3].StartCallCount);
        }

        [Test]
        public async Task InvokeAsync_IndexBeyondFourPlayers_WrapsAroundToCorrectGameState()
        {
            await this.step.InvokeAsync(this.BuildArgs(currentGameStateIndex: 5));

            Assert.AreEqual(1, this.gameStates[1].StartCallCount);
        }

        [Test]
        public async Task InvokeAsync_AdvancesToHandleCardPlayedStepAndWaitsForPlayCardEvent()
        {
            await this.step.InvokeAsync(this.BuildArgs(currentGameStateIndex: 0));

            Assert.AreEqual(nameof(HandleCardPlayedStep), this.workflow.NextStepName);
            CollectionAssert.Contains(this.workflow.EventsWaitedForWithData, EventNames.PlayCard);
        }
    }
}
