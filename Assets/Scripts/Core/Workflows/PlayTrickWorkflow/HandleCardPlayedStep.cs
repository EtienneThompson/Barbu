namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using UnityEngine;
    using Zenject;

    public class HandleCardPlayedStep : IStep<PlayTrickArguments>
    {
        public class Factory : PlaceholderFactory<HandleCardPlayedStep>
        {
        }

        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;

        public HandleCardPlayedStep(IStateMachine stateMachine, ITelemetryService telemetryService)
        {
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
                args.Workflow.SetNextStep(nameof(ResolveTrickStep));
            } else
            {
                this.telemetryService.LogInfo("[PlayTrickWorkflow] Moving to play next card...");
                args.Workflow.SetNextStep(nameof(PlayCardStep));
            }

            return Task.CompletedTask;
        }
    }
}
