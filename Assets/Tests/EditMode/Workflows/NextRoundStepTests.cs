namespace Barbu.Tests.EditMode.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;
    using PlayTrickWorkflow = Barbu.Core.Workflows.PlayTrickWorkflow.PlayTrickWorkflow;

    public class NextRoundStepTests
    {
        private FakeInGamePointsController inGamePointsController;
        private FakeWorkflow workflow;
        private NextRoundStep step;

        [SetUp]
        public void SetUp()
        {
            this.inGamePointsController = new FakeInGamePointsController();
            this.workflow = new FakeWorkflow();
            this.step = new NextRoundStep(this.inGamePointsController, new FakeTelemetryService());
        }

        private static RoundArguments BuildData(int currentRoundIndex, int totalRounds, PlayTrickWorkflow playTrickWorkflow = null)
        {
            var rounds = new List<IRound>();
            for (int i = 0; i < totalRounds; i++)
            {
                rounds.Add(new HeartsRound());
            }

            return new RoundArguments
            {
                CurrentRoundIndex = currentRoundIndex,
                Rounds = rounds,
                PlayTrickWorkflow = playTrickWorkflow,
            };
        }

        [Test]
        public async Task InvokeAsync_ActivatesInGamePointsOverlay()
        {
            // SetActive(true) happens unconditionally before the final-round branch,
            // so the final-round config is used here to avoid needing a real
            // PlayTrickWorkflow (which the not-final-round branch disposes).
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = BuildData(currentRoundIndex: 1, totalRounds: 2) };

            await this.step.InvokeAsync(args);

            CollectionAssert.Contains(this.inGamePointsController.ActiveStates, true);
        }

        [Test]
        public async Task InvokeAsync_OnFinalRound_AdvancesToCompleteGameStep()
        {
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = BuildData(currentRoundIndex: 1, totalRounds: 2) };

            await this.step.InvokeAsync(args);

            Assert.AreEqual(nameof(CompleteGameStep), this.workflow.NextStepName);
        }

        [Test]
        public async Task InvokeAsync_NotFinalRound_DisposesPlayTrickWorkflowAndAdvancesToSetupRound()
        {
            var playTrickWorkflow = new PlayTrickWorkflow(
                new FakeEventsController(),
                new FakeStateMachine(),
                new FakeTelemetryService(),
                new FakeComputerStateFactory(),
                new PlayCardStep.Factory(),
                new HandleCardPlayedStep.Factory(),
                new ResolveTrickStep.Factory(),
                new HeartsRound(),
                new Dictionary<string, int[]>
                {
                    [Constants.PlayerIds.Player1] = new int[2],
                    [Constants.PlayerIds.Player2] = new int[2],
                    [Constants.PlayerIds.Player3] = new int[2],
                    [Constants.PlayerIds.Player4] = new int[2],
                },
                new[] { new Hand(), new Hand(), new Hand(), new Hand() },
                startingPlayer: 0,
                roundNumber: 0);
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = BuildData(currentRoundIndex: 0, totalRounds: 2, playTrickWorkflow) };

            await this.step.InvokeAsync(args);

            Assert.IsNull(args.Data.PlayTrickWorkflow);
            Assert.AreEqual(1, args.Data.CurrentRoundIndex);
            Assert.AreEqual(nameof(SetupRoundStep), this.workflow.NextStepName);
        }
    }
}
