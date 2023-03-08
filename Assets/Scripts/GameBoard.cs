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
    private Hand[] hands = new Hand[4];

    private RoundManager roundManager;

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

        ResetRound();

        this.roundManager = new RoundManager(hands);

        this.stateMachine.SetCardPlayable(true);
    }

    public void ResetRound()
    {
        this.Shuffle(cards);
        this.DealHand(cards);
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
        for (int i = 0; i < 4; i++)
        {
            Hand hand = new Hand();
            hands[i] = hand;
        }

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

                GameObject myCard = Instantiate(Resources.Load("BlankPlayingCard", typeof(GameObject)), position, Quaternion.identity) as GameObject;
                Card card = myCard.GetComponent<Card>();
                var index = i * 4 + j;
                var suit = cards[index].Substring(0, cards[index].Length - 2);
                var rank = cards[index].Substring(cards[index].Length - 2);
                var playerId = (j + 1).ToString();
                card.Initialize(suit, rank, playerId, rotateX, rotateY, rotateZ);

                hands[j].AddCard(card);
            }
        }
    }
}
