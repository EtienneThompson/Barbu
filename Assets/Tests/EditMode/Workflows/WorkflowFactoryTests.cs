namespace Barbu.Tests.EditMode.Workflows
{
    using System;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    // CreateTraditionalRoundWorkflow/CreateSingleRoundWorkflow(valid type)/
    // CreateChaosRoundWorkflow all end up constructing a RoundWorkflow, whose
    // constructor does GameObject.Find(Constants.GameObjects.GameBoard) and then
    // calls into the real Unity Ads SDK via AdvertisementController - a scene +
    // ads dependency that's out of scope for EditMode unit tests. Only the
    // validation branch that runs before that point is covered here.
    public class WorkflowFactoryTests
    {
        [Test]
        public void CreateSingleRoundWorkflow_UnknownRoundType_ThrowsArgumentException()
        {
            var factory = new WorkflowFactory(
                new FakeTelemetryService(),
                new PlayTrickWorkflow.Factory(),
                new RoundWorkflow.Factory(),
                new FakeRandomService());

            Assert.Throws<ArgumentException>(() => factory.CreateSingleRoundWorkflow("NotARealRound"));
        }
    }
}
