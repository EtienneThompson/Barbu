namespace Barbu.Core.Workflows
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Interfaces.Rounds;

    public class WorkflowFactory : IWorkflowFactory
    {
        private readonly IEventsController eventsController;
        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;
        private readonly IComputerStateFactory computerStateFactory;

        public WorkflowFactory(
            IEventsController eventsController, IStateMachine stateMachine, ITelemetryService telemetryService, IComputerStateFactory computerStateFactory)
        {
            this.eventsController = eventsController;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
            this.computerStateFactory = computerStateFactory;
        }

        public IWorkflow CreatePlayTrickWorkflow(IRound round, Dictionary<string, int[]> playerPoints, Hand[] playerHands, int startingPlayer, int roundNumber)
        {
            return new PlayTrickWorkflow.PlayTrickWorkflow(
                this.eventsController,
                this.stateMachine,
                this.telemetryService,
                this.computerStateFactory,
                round,
                playerPoints,
                playerHands,
                startingPlayer,
                roundNumber);
        }

        public IWorkflow CreateRoundWorkflow(GameTypes gameType, List<IRound> rounds)
        {
            return new RoundWorkflow.RoundWorkflow(
                this.eventsController,
                this.stateMachine,
                this.telemetryService,
                this.computerStateFactory,
                gameType,
                rounds);
        }
    }
}
