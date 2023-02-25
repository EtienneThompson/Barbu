using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private StateMachine stateMachine;
    private string[] kCardSuits = new string[] { "Club", "Diamond", "Spade", "Heart" };
    // Ranks must match the resources used, 01 is A and 11, 12, and 13 are J, Q, and K.
    private string[] kCardRanks = new string[] { "01", "02", "03", "04", "05", "06", "07",
                                                 "08", "09", "10", "11", "12", "13" };
    private string[] cards = new string[52];
    private Card[,] hands = new Card[4, 13];

    private const int cardsPerPile = 4;
    private Card[] currentPile = new Card[cardsPerPile];
    private int numCardsInPile = 0;

    public static GameBoard instance;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        instance = this;

        stateMachine = new StateMachine();

        // Create the deck of 52 cards.
        for (int i = 0; i < kCardSuits.Length; i++)
        {
            for (int j = 0; j < kCardRanks.Length; j++)
            {
                var card = kCardSuits[i] + kCardRanks[j];
                cards[i * 13 + j] = card;
            }
        }

        Shuffle(cards);
        DealHand(cards);

        // Listen for events when cards are being played.
        Card.onPlayed += this.OnCardPlayed;

        this.stateMachine.SetCardPlayable(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < 5; i++)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = (int)Mathf.Floor(Random.value * (n--));
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }

    private void DealHand<T>(T[] deck)
    {

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Debug.Log($"Rendering card {i} for player {j}");
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

                GameObject myCard = Instantiate(Resources.Load("BlankPlayingCard", typeof(GameObject)), position, Quaternion.identity) as GameObject;
                Card card = myCard.GetComponent<Card>();
                var index = i * 4 + j;
                var suit = cards[index].Substring(0, cards[index].Length - 2);
                var rank = cards[index].Substring(cards[index].Length - 2);
                card.Initialize(suit, rank, rotateX, rotateY, rotateZ);

                hands[j, i] = card;
            }
        }
    }

    private T GetRandomFromArray<T>(T[] array)
    {
        return array[(int)Mathf.Floor(Random.value * array.Length)];
    }

    private void OnCardPlayed(Card card)
    {
        this.stateMachine.SetCardPlayable(false);
        
        this.currentPile[this.numCardsInPile] = card;
        this.numCardsInPile++;

        if (this.numCardsInPile == cardsPerPile) {
            this.ResolvePile();
        }

        this.stateMachine.SetCardPlayable(true);
    }

    private void ResolvePile()
    {
        for (int i = 0; i < this.numCardsInPile; i++) {
            this.currentPile[i].gameObject.SetActive(false);
            this.currentPile[i].GetComponent<Renderer>().enabled = false;
            this.currentPile[i] = null;
        }

        this.numCardsInPile = 0;
    }
}
