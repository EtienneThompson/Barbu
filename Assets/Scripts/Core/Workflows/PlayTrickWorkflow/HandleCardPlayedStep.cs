namespace Barbu
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Gameplay;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;
    using UnityEngine;

    public class HandleCardPlayedStep : IStep<PlayTrickArguments>
    {
        private IWorkflow workflow;
        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<PlayTrickArguments> args)
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Handling played card...");
            var card = (Card)args.EventData;
            this.telemetryService.LogInfo($"[PlayTrickWorkflow] Card player id: {card.playerId}");
            this.stateMachine.SetCardPlayable(false);

            var gameState = args.Data.gameStates[args.Data.currentGameStateIndex % 4];
            gameState.CleanUp();

            var previousHighCard = args.Data.currentPile.GetHighestCard();
            previousHighCard?.RemoveHighlight();

            args.Data.currentPile.AddCardToPile(card);
            var newHighestCard = args.Data.currentPile.GetHighestCard();
            newHighestCard.Highlight(Color.yellow);
            this.stateMachine.SetHighestRank(newHighestCard.rank);

            args.Data.currentGameStateIndex++;
            if (args.Data.currentPile.GetPileSize() == 4)
            {
                this.telemetryService.LogInfo("[PlayTrickWorkflow] Moving to resolve pile...");
                this.workflow.SetNextStep(nameof(ResolveTrickStep));
            } else
            {
                this.telemetryService.LogInfo("[PlayTrickWorkflow] Moving to play next card...");
                this.workflow.SetNextStep(nameof(PlayCardStep));
            }

            return Task.CompletedTask;
        }
    }
}
