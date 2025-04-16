namespace Barbu.Gameplay
{
    using System;
    using System.Collections;
    using Barbu.Core;
    using Barbu.Models;
    using UnityEngine;

    public class Card : MonoBehaviour
    {
        public enum CardState
        {
            Waiting,
            Played,
        }

        public const float baseSpeed = 30.0f;
        public const float MoveToPlayerMultiplier = 2.0f;
        public const float AdjustPositionInHandMultiplier = 0.75f;

        public string suit;
        public int rank;
        public string playerId;
        public CardState state;
        private StateMachine stateMachine;
        private EventsController eventsController;
        private Renderer meshRenderer;
        private Color initialColor;
        private string resourceRank;

        public void InitializeData(string suit, string rank, string playerId)
        {
            this.playerId = playerId;
            this.suit = suit;
            this.resourceRank = rank;
            this.rank = System.Int32.Parse(rank);
            if (this.rank == 1)
            {
                // Handle case where Ace is lowest in the resource pack, but highest rank in the game.
                this.rank = 14;
            }
        }

        public void InitializeGameObject(Vector3 position, float rotateX, float rotateY, float rotateZ)
        {
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.state = CardState.Waiting;
            this.stateMachine = new StateMachine();
            this.eventsController = EventsController.GetInstance();

            string path = "PlayingCards/Resource/Materials/BackColor_Black/Black_PlayingCards_" + this.suit + this.resourceRank + "_00";
            this.meshRenderer.material = Resources.Load(path, typeof(Material)) as Material;
            transform.position = position;
            transform.Rotate(rotateX, rotateY, rotateZ, Space.Self);
            this.meshRenderer.material.SetFloat("_BorderThickness", 0.0f);
            this.initialColor = this.meshRenderer.material.GetColor("_BorderColor");
        }

        // Update is called once per frame
        void Update()
        {
            if (((this.stateMachine.MustPlayCardInStartingSuit() && this.suit.Equals(this.stateMachine.GetStartingSuit())) ||
                !this.stateMachine.MustPlayCardInStartingSuit()) &&
                this.stateMachine.IsCardPlayable() &&
                !this.stateMachine.IsMenuOpen() &&
                Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        this.PlayCard();
                    }
                }
            }
        }

        public string GetName()
        {
            return this.suit + this.rank.ToString();
        }

        public int GetSortingRank(Settings.SortingOptions option)
        {
            switch (option)
            {
                case Settings.SortingOptions.HighToLow:
                    return -1 * this.GetRankSortingRank();
                case Settings.SortingOptions.LowToHigh:
                    return this.GetRankSortingRank();
                case Settings.SortingOptions.SuitHighToLow:
                    return -1 * (this.GetSuitSortingRank() + this.GetRankSortingRank());
                case Settings.SortingOptions.SuitLowToHigh:
                    return this.GetSuitSortingRank() + this.GetRankSortingRank();
                case Settings.SortingOptions.SuitHighToLowAlternating:
                    return this.GetSuitSortingRank(alternating: true) + this.GetRankSortingRank();
                case Settings.SortingOptions.SuitLowToHighAlternating:
                    return this.GetSuitSortingRank(alternating: true) + this.GetRankSortingRank();
                case Settings.SortingOptions.None:
                default:
                    return 0;
            }
        }

        public void PlayCard()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            stateMachine.IncrementNumCardsPlayed();
            StartCoroutine(MoveToCenterRoutine());
            this.state = CardState.Played;
            this.stateMachine.SetCardPlayable(false);
        }

        public void Highlight(Color color)
        {
            this.meshRenderer.material.SetFloat("_BorderThickness", 0.05f);
            this.meshRenderer.material.SetColor("_BorderColor", color);
        }

        public void RemoveHighlight()
        {
            this.meshRenderer.material.SetFloat("_BorderThickness", 0.0f);
            this.meshRenderer.material.SetColor("_BorderColor", initialColor);
        }

        public void StartPileResolution(string player)
        {
            StartCoroutine(MoveTowardsPlayer(player));
        }

        public void StartPositionAdjustment(int index, int cardsInHand)
        {
            StartCoroutine(AdjustPositionInHand(index, cardsInHand));
        }

        private int GetSuitSortingRank(bool alternating = false)
        {
            switch (this.suit)
            {
                case Constants.CardSuits.Heart:
                    return 0;
                case Constants.CardSuits.Diamond:
                    return 20 * (alternating ? 2 : 1);
                case Constants.CardSuits.Spade:
                    return (int)(40.0f * (alternating ? 0.5f : 1.0f));
                case Constants.CardSuits.Club:
                    return 60;
                default:
                    return 80;
            }
        }

        private int GetRankSortingRank()
        {
            return this.rank;
        }

        private float GetMovingSpeed()
        {
            return baseSpeed * (this.stateMachine.IsAutoPlayMode() ? 10.0f : 1.0f) * Time.deltaTime;
        }

        private float GetPlayCardDelay()
        {
            return 0.5f * (this.stateMachine.IsAutoPlayMode() ? 0.1f : 1.0f);
        }

        private IEnumerator MoveToCenterRoutine()
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            Vector3 center;
            var verticalPosition = 0.5f + this.stateMachine.NumCardsPlayed() * 0.01f;
            switch (this.playerId)
            {
                case Constants.PlayerIds.Player1:
                    center = new Vector3(0.0f, verticalPosition, -3.0f);
                    break;
                case Constants.PlayerIds.Player2:
                    center = new Vector3(-3.0f, verticalPosition, 0.0f);
                    break;
                case Constants.PlayerIds.Player3:
                    center = new Vector3(0.0f, verticalPosition, 3.0f);
                    break;
                case Constants.PlayerIds.Player4:
                    center = new Vector3(3.0f, verticalPosition, 0.0f);
                    break;
                default:
                    throw new Exception("This card has an invalid player id " + this.playerId);
            }

            var rotate = new Vector3(0.0f, UnityEngine.Random.Range(-5.0f, 5.0f), 0.0f);
            while (transform.position != center)
            {
                transform.position = Vector3.MoveTowards(transform.position, center, this.GetMovingSpeed());
                transform.Rotate(rotate * this.GetMovingSpeed());
                yield return null;
            }
            yield return new WaitForSeconds(this.GetPlayCardDelay());
            eventsController.Fire(EventNames.PlayCard, this);
        }

        private IEnumerator MoveTowardsPlayer(string player)
        {
            Vector3 finalPosition;
            switch (player)
            {
                case Constants.PlayerIds.Player1:
                    finalPosition = new Vector3(0, 0.25f, -25);
                    break;
                case Constants.PlayerIds.Player2:
                    finalPosition = new Vector3(-50, 0.25f, 0);
                    break;
                case Constants.PlayerIds.Player3:
                    finalPosition = new Vector3(0, 0.25f, 25);
                    break;
                case Constants.PlayerIds.Player4:
                    finalPosition = new Vector3(50, 0.25f, 0);
                    break;
                default:
                    throw new Exception("The provided player id is invalid");
            }

            while (transform.position != finalPosition)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    finalPosition,
                    this.GetMovingSpeed() * MoveToPlayerMultiplier);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            this.gameObject.SetActive(false);
            this.meshRenderer.enabled = false;
            this.eventsController.Fire(EventNames.CardInPileResolved);
        }

        private IEnumerator AdjustPositionInHand(float index, float cardsInHand)
        {
            Vector3 finalPosition;
            switch (this.playerId)
            {
                case Constants.PlayerIds.Player1:
                    finalPosition = new Vector3((index * 2) - (cardsInHand - 1), 0.5f + (0.01f * index), -12);
                    break;
                case Constants.PlayerIds.Player2:
                    finalPosition = new Vector3(-22, 0.5f + (-0.01f * index), index - (cardsInHand / 2));
                    break;
                case Constants.PlayerIds.Player3:
                    finalPosition = new Vector3(index - (cardsInHand / 2), 0.5f + (-0.01f * index), 12);
                    break;
                case Constants.PlayerIds.Player4:
                    finalPosition = new Vector3(22, 0.5f + (0.01f * index), index - (cardsInHand / 2));
                    break;
                default:
                    throw new Exception("The provided player id is invalid");
            }

            while (transform.position != finalPosition)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    finalPosition,
                    this.GetMovingSpeed() * AdjustPositionInHandMultiplier);
                yield return null;
            }
        }
    }
}
