namespace Barbu.Tests.EditMode.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class ShowAdvertisementStepTests
    {
        private FakeRoundOverlayController roundOverlayController;
        private FakeAdvertisementController advertisementController;
        private FakeWorkflow workflow;
        private ShowAdvertisementStep step;

        [SetUp]
        public void SetUp()
        {
            this.roundOverlayController = new FakeRoundOverlayController();
            this.advertisementController = new FakeAdvertisementController();
            this.workflow = new FakeWorkflow();
            this.step = new ShowAdvertisementStep(this.roundOverlayController, this.advertisementController, new FakeTelemetryService());
        }

        [Test]
        public async Task InvokeAsync_HidesRoundOverlay()
        {
            await this.step.InvokeAsync(new StepArguments<RoundArguments>(this.workflow) { Data = new RoundArguments() });

            CollectionAssert.Contains(this.roundOverlayController.ActiveStates, false);
            Assert.AreEqual(1, this.roundOverlayController.HideTextCallCount);
        }

        [Test]
        public async Task InvokeAsync_ShowsInterstitialAd()
        {
            await this.step.InvokeAsync(new StepArguments<RoundArguments>(this.workflow) { Data = new RoundArguments() });

            Assert.AreEqual(1, this.advertisementController.ShowInterstitialAdCallCount);
        }

        [Test]
        public async Task InvokeAsync_EndsTheWorkflow()
        {
            await this.step.InvokeAsync(new StepArguments<RoundArguments>(this.workflow) { Data = new RoundArguments() });

            Assert.IsNull(this.workflow.NextStepName);
        }
    }
}
