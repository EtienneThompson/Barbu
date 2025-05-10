namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;

    public class PlayCardStep : IStep<PlayTrickArguments>
    {
        private IWorkflow parentWorkflow;
        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.parentWorkflow = workflow;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<PlayTrickArguments> args)
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Playing card...");
            var gameState = args.Data.gameStates[args.Data.currentGameStateIndex % 4];
            this.telemetryService.LogInfo($"[PlayTrickWorkflow] Player {gameState.PlayerId} playing...");
            this.stateMachine.SetCardPlayable(true);
            gameState.Start();
            this.parentWorkflow.SetNextStep(nameof(HandleCardPlayedStep));
            this.parentWorkflow.WaitForEventWithData(EventNames.PlayCard);

            return Task.CompletedTask;
        }
    }
}