namespace Barbu
{
    using Barbu.Gameplay;
    using UnityEngine;

    public class CardFactory : MonoBehaviour
    {
        public static Card CreateCard(string suit, string rank, string playerId)
        {
            GameObject cardObject = Instantiate(
                Resources.Load("BlankPlayingCard", typeof(GameObject)),
                new Vector3(0, 0, 0),
                Quaternion.identity) as GameObject;
            Card card = cardObject.GetComponent<Card>();

            card.InitializeData(suit, rank, playerId);
            return card;
        }
    }
}
