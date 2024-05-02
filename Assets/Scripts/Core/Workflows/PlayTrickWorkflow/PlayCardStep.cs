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
        private ITelemetryService telemetryService;

        public void Initialize(IWorkflow workflow, ITelemetryService telemetryService)
        {
            this.parentWorkflow = workflow;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<PlayTrickArguments> args)
        {
            if (args.Data.cardsPlayed < 4)
            {
                this.telemetryService.LogInfo("[PlayTrickWorkflow] Playing card...");
                args.Data.cardsPlayed++;
                var gameState = args.Data.gameStates[args.Data.currentGameStateIndex++ % 4];
                this.telemetryService.LogInfo($"[PlayTrickWorkflow] Player {gameState.PlayerId} playing...");
                // gameState.Start();
                this.parentWorkflow.SetNextStep(nameof(HandleCardPlayedStep));
                this.parentWorkflow.WaitForEventWithData(EventNames.PlayCard);
            }
            else
            {
                this.telemetryService.LogInfo("[PlayTrickWorkflow] Moving to resolve trick step");
                this.parentWorkflow.SetNextStep(nameof(ResolveTrickStep));
            }

            return Task.CompletedTask;
        }
    }
}