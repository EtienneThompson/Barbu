namespace Barbu
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;

    public class NextRoundStep : IStep<RoundArguments>
    {
        private IWorkflow workflow;
        private StateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(IWorkflow workflow, StateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            if (args.Data.CurrentRoundIndex + 1 == args.Data.Rounds.Count)
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Rounds are complete, moving to complete game step...");
                this.workflow.SetNextStep(nameof(CompleteGameStep));
            }
            else
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Configuring next round...");
                args.Data.PlayTrickWorkflow.Dispose();
                args.Data.PlayTrickWorkflow = null;
                args.Data.CurrentRoundIndex++;
                this.workflow.SetNextStep(nameof(SetupRoundStep));
            }

            return Task.CompletedTask;
        }
    }
}
