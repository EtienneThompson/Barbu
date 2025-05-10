namespace Barbu
{
    using System;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;
    using UnityEngine;
    using Zenject;

    public class GameBoard : MonoBehaviour
    {
        private IStateMachine stateMachine;
        private GlobalContext globalContext;
        private ITelemetryService telemetryService;
        private IRoundFactory roundFactory;
        private ICardFactory cardFactory;
        private string[] kCardSuits = new string[] { "Club", "Diamond", "Spade", "Heart" };
        // Ranks must match the resources used, 01 is A and 11, 12, and 13 are J, Q, and K.
        private string[] kCardRanks = new string[] { "01", "02", "03", "04", "05", "06", "07",
                                                 "08", "09", "10", "11", "12", "13" };
        private string[] cards = new string[52];
        private Hand[] hands = new Hand[4];

        [Inject]
        public void Init(
            ITelemetryService telemetryService,
            IStateMachine stateMachine,
            IRoundFactory roundFactory,
            ICardFactory cardFactory)
        {
            this.telemetryService = telemetryService;
            this.stateMachine = stateMachine;
            this.roundFactory = roundFactory;
            this.cardFactory = cardFactory;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.globalContext = GlobalContext.GetInstance();

            // Create the deck of 52 cards.
            for (int i = 0; i < kCardSuits.Length; i++)
            {
                for (int j = 0; j < kCardRanks.Length; j++)
                {
                    var card = kCardSuits[i] + kCardRanks[j];
                    cards[i * 13 + j] = card;
                }
            }

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
                    this.globalContext.RoundWorkflow = this.roundFactory.CreateTraditionalRoundWorkflow();
                    Statistics.IncrementGamesPlayed(GameTypes.Traditional);
                    break;
                case Constants.SingleRoundManager.GameName:
                    this.globalContext.RoundWorkflow = this.roundFactory.CreateSingleRoundWorkflow(subType);
                    Statistics.IncrementGamesPlayed(GameTypes.Single);
                    break;
                case Constants.ChaosRoundManager.GameName:
                    this.globalContext.RoundWorkflow = this.roundFactory.CreateChaosRoundWorkflow();
                    Statistics.IncrementGamesPlayed(GameTypes.Chaos);
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

        private void DealHand()
        {
            this.Shuffle(cards);
            this.DealHand(cards);
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

        private void Shuffle<T>(T[] array)
        {
            for (int i = 0; i < 5; i++)
            {
                int n = array.Length;
                while (n > 1)
                {
                    int k = (int)Mathf.Floor(UnityEngine.Random.value * (n--));
                    T temp = array[n];
                    array[n] = array[k];
                    array[k] = temp;
                }
            }
        }

        private void DealHand(string[] deck)
        {
            for (int i = 0; i < 4; i++)
            {
                hands[i] = new Hand();
            }

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var index = i * 4 + j;
                    var suit = deck[index].Substring(0, deck[index].Length - 2);
                    var rank = deck[index].Substring(deck[index].Length - 2);
                    var playerId = (j + 1).ToString();
                    var card = this.cardFactory.CreateCard(suit, rank, playerId);

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
                    }
                    else if (j == 1)
                    {
                        position = new Vector3(-22, 0.5f + (-0.01f * i), i - 6);
                        rotateY = 90.0f;
                        rotateZ = 180.0f;
                    }
                    else if (j == 2)
                    {
                        position = new Vector3(i - 6, 0.5f + (-0.01f * i), 12);
                        rotateZ = 180.0f;
                    }
                    else
                    {
                        position = new Vector3(22, 0.5f + (0.01f * i), i - 6);
                        rotateY = -90.0f;
                        rotateZ = 180.0f;
                    }

                    var cardToInit = hands[j].GetCardAtPosition(i);
                    cardToInit.InitializeGameObject(position, rotateX, rotateY, rotateZ);
                }
            }
        }
    }
}
