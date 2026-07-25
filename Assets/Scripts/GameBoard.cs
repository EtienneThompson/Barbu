namespace Barbu
{
    using System;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows;
    using Barbu.Gameplay;
    using UnityEngine;
    using Zenject;

    public class GameBoard : MonoBehaviour, IGameBoard
    {
        private IStateMachine stateMachine;
        private GlobalContext globalContext;
        private ITelemetryService telemetryService;
        private IWorkflowFactory workflowFactory;
        private IDeck deck;
        private IStatisticsService statisticsService;
        private Hand[] hands = new Hand[4];

        [Inject]
        public void Init(
            ITelemetryService telemetryService,
            IStateMachine stateMachine,
            IWorkflowFactory workflowFactory,
            IDeck deck,
            IStatisticsService statisticsService)
        {
            telemetryService.LogInfo("GameBoard injected");
            this.telemetryService = telemetryService;
            this.stateMachine = stateMachine;
            this.workflowFactory = workflowFactory;
            this.deck = deck;
            this.statisticsService = statisticsService;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.globalContext = GlobalContext.GetInstance();
            this.CreateNewGame(Constants.TraditionalRoundManager.GameName, string.Empty);
        }

        public void CreateNewGame(string gameName, string subType)
        {
            this.telemetryService.LogInfo("Creating new game...");
            this.stateMachine.SetIsSettingUp(true);
            this.stateMachine.SetCardPlayable(false);
            this.stateMachine.ResetNumCardsPlayed();
            this.globalContext.RoundWorkflow?.Dispose();
            this.CleanupRound();
            switch (gameName)
            {
                case Constants.TraditionalRoundManager.GameName:
                    this.globalContext.RoundWorkflow = this.workflowFactory.CreateTraditionalRoundWorkflow();
                    this.statisticsService.IncrementGamesPlayed(GameTypes.Traditional);
                    break;
                case Constants.SingleRoundManager.GameName:
                    this.globalContext.RoundWorkflow = this.workflowFactory.CreateSingleRoundWorkflow(subType);
                    this.statisticsService.IncrementGamesPlayed(GameTypes.Single);
                    break;
                case Constants.ChaosRoundManager.GameName:
                    this.globalContext.RoundWorkflow = this.workflowFactory.CreateChaosRoundWorkflow();
                    this.statisticsService.IncrementGamesPlayed(GameTypes.Chaos);
                    break;
                default:
                    throw new Exception("Incorrect game name provided");
            }

            this.stateMachine.SetIsSettingUp(false);
            Task _ = this.globalContext.RoundWorkflow.StartAsync();
        }

        public void CleanupRound()
        {
            this.DestroyCards();
        }

        public Hand[] SetupRound()
        {
            this.DealHand();
            return this.hands;
        }

        private void DestroyCards()
        {
            foreach (var hand in this.hands)
            {
                if (hand == null)
                {
                    continue;
                }

                foreach (var card in hand.GetHand())
                {
                    if (card != null)
                    {
                        Destroy(card.gameObject);
                    }
                }
            }
        }

        private void DealHand()
        {
            this.deck.ResetDeck();
            this.deck.ShuffleComplete();

            for (int i = 0; i < 4; i++)
            {
                hands[i] = new Hand();
            }

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var card = this.deck.DrawCard((j + 1).ToString());
                    hands[j].AddCard(card);
                }
            }

            hands[0].SortHand(Settings.SortingPreference);

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 position;
                    float rotateX = 0.0f;
                    float rotateY = 0.0f;
                    float rotateZ = 0.0f;
                    if (j == 0)
                    {
                        position = new Vector3((i * 2) - 12, 0.5f + (0.01f * i), -12);
                        rotateX = -90.0f;
                    }
                    else if (j == 1)
                    {
                        position = new Vector3(-22, 0.5f + (-0.01f * i), i - 6.5f);
                        rotateY = 90.0f;
                        rotateZ = 90.0f;
                    }
                    else if (j == 2)
                    {
                        position = new Vector3(i - 6.5f, 0.5f + (-0.01f * i), 12);
                        rotateX = -90.0f;
                        rotateZ = 180.0f;
                    }
                    else
                    {
                        position = new Vector3(22, 0.5f + (0.01f * i), i - 6.5f);
                        rotateY = -90.0f;
                        rotateZ = -90.0f;
                    }

                    var cardToInit = hands[j].GetCardAtPosition(i);
                    cardToInit.InitializeGameObject(position, rotateX, rotateY, rotateZ);
                }
            }
        }
    }
}
