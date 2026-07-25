namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Zenject;

    public class PlayCardStep : IStep<PlayTrickArguments>
    {
        public class Factory : PlaceholderFactory<PlayCardStep>
        {
        }

        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;

        public PlayCardStep(IStateMachine stateMachine, ITelemetryService telemetryService)
        {
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
            args.Workflow.SetNextStep(nameof(HandleCardPlayedStep));
            args.Workflow.WaitForEventWithData(EventNames.PlayCard);

            return Task.CompletedTask;
        }
    }
}
