using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private string[] kCardSuits = new string[] { "Club", "Diamond", "Spade", "Heart" };
    // Ranks must match the resources used, 01 is A and 11, 12, and 13 are J, Q, and K.
    private string[] kCardRanks = new string[] { "01", "02", "03", "04", "05", "06", "07",
                                                 "08", "09", "10", "11", "12", "13" };
    private string[] cards = new string[52];

    private Card[] hand = new Card[13];

    public static GameBoard instance;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Debug.Log("Hello world!");
        instance = this;

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
            Vector3 position = new Vector3((i - 6) * 10, (i / 10.0f) + 1, -100);
            GameObject myCard = Instantiate(Resources.Load("BlankPlayingCard", typeof(GameObject)), position, Quaternion.identity) as GameObject;
            Card card = myCard.GetComponent<Card>();
            var suit = cards[i].Substring(0, cards[i].Length - 2);
            var rank = cards[i].Substring(cards[i].Length - 2);
            card.Initialize(suit, rank, true);

            hand[i] = card;
        }
    }

    private T GetRandomFromArray<T>(T[] array)
    {
        return array[(int)Mathf.Floor(Random.value * array.Length)];
    }
}
