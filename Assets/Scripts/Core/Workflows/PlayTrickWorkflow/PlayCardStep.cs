namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models;
    using Barbu.Models.Workflows;

    public class PlayCardStep : IStep<PlayTrickArguments>
    {
        private IWorkflow parentWorkflow;
        private StateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(IWorkflow workflow, StateMachine stateMachine, ITelemetryService telemetryService)
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