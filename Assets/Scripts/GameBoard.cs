using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameBoard : MonoBehaviour
{
    private StateMachine stateMachine;
    private string[] kCardSuits = new string[] { "Club", "Diamond", "Spade", "Heart" };
    // Ranks must match the resources used, 01 is A and 11, 12, and 13 are J, Q, and K.
    private string[] kCardRanks = new string[] { "01", "02", "03", "04", "05", "06", "07",
                                                 "08", "09", "10", "11", "12", "13" };
    private string[] cards = new string[52];
    private Hand[] hands = new Hand[4];
    private IRoundManager roundManager;
    private ScoreMenu scoreMenu;
    private InGamePointsController inGamePointsController;

    public static GameBoard instance;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        instance = this;

        this.stateMachine = new StateMachine();

        // Create the deck of 52 cards.
        for (int i = 0; i < kCardSuits.Length; i++)
        {
            for (int j = 0; j < kCardRanks.Length; j++)
            {
                var card = kCardSuits[i] + kCardRanks[j];
                cards[i * 13 + j] = card;
            }
        }

        // Hide any UI objects.
        GameObject scoreBoard = GameObject.Find(Constants.GameObjects.ScoreMenuCanvas);
        scoreBoard.SetActive(false);
        GameObject gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
        gamesMenu.SetActive(false);
        GameObject settingsMenu = GameObject.Find(Constants.GameObjects.SettingsMenu);
        settingsMenu.SetActive(false);

        if (Settings.HasSeenHowToPlayByDefault())
        {
            GameObject howToPlayScreen = GameObject.Find(Constants.GameObjects.HowToPlayScreen);
            howToPlayScreen.SetActive(false);
        }
        else
        {
            Settings.SetSeenHowToPlayByDefault();
        }

        this.scoreMenu = scoreBoard.GetComponent<ScoreMenu>();
        GameObject inGamePoints = GameObject.Find(Constants.GameObjects.InGamePoints);
        this.inGamePointsController = inGamePoints.GetComponent<InGamePointsController>();

        this.DealHand();
        this.roundManager = new TraditionalRoundManager(this, this.scoreMenu, this.inGamePointsController, this.hands);
        Statistics.IncrementGamesPlayed(Statistics.GameTypes.Traditional);
        this.roundManager.PreRound();
    }

    public void CreateNewGame(string gameName)
    {
        this.stateMachine.SetCardPlayable(false);
        this.stateMachine.ResetNumCardsPlayed();
        this.inGamePointsController.ResetPoints();
        this.roundManager.Destroy();
        this.roundManager = null;
        this.CleanupRound();
        this.DealHand();
        switch (gameName)
        {
            case Constants.TraditionalRoundManager.GameName:
                this.roundManager = new TraditionalRoundManager(this, this.scoreMenu, this.inGamePointsController, this.hands);
                Statistics.IncrementGamesPlayed(Statistics.GameTypes.Traditional);
                break;
            case Constants.SingleRoundManager.GameName:
                this.roundManager = new SingleRoundManager(this, this.scoreMenu, this.inGamePointsController, this.hands);
                Statistics.IncrementGamesPlayed(Statistics.GameTypes.Single);
                break;
            case Constants.ChaosRoundManager.GameName:
                this.roundManager = new ChaosRoundManager(this, this.scoreMenu, this.inGamePointsController, this.hands);
                Statistics.IncrementGamesPlayed(Statistics.GameTypes.Chaos);
                break;
            default:
                throw new Exception("Incorrect game name provided");
        }
        this.roundManager.PreRound();
    }

    public void CleanupRound()
    {
        this.DestroyCards();
    }

    public void SetupRound()
    {
        this.DealHand();
        this.stateMachine.SetCardPlayable(true);
        this.stateMachine.SetStartingSuit("");
        this.roundManager.NextRound(this.hands);
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
                GameObject myCard = Instantiate(Resources.Load("BlankPlayingCard", typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                Card card = myCard.GetComponent<Card>();
                var index = i * 4 + j;
                var suit = deck[index].Substring(0, deck[index].Length - 2);
                var rank = deck[index].Substring(deck[index].Length - 2);
                var playerId = (j + 1).ToString();
                card.InitializeData(suit, rank, playerId);

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
                    position = new Vector3((i - 6) * 10, (i / 10.0f) + 1, -100);
                    rotateX = -45.0f;
                }
                else if (j == 1)
                {
                    position = new Vector3(-90, 10, (i - 8) * 10);
                    rotateY = 90.0f;
                    rotateX = -70.0f;
                }
                else if (j == 2)
                {
                    position = new Vector3((i - 6) * 10, 10, 40);
                    rotateX = 90.0f;
                }
                else
                {
                    position = new Vector3(90, 10, (i - 8) * 10);
                    rotateY = -90.0f;
                    rotateX = -70.0f;
                }

                var cardToInit = hands[j].GetCardAtPosition(i);
                cardToInit.InitializeGameObject(position, rotateX, rotateY, rotateZ);
            }
        }
    }
}
