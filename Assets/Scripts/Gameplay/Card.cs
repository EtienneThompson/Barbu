using System;
using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardState
    {
        Waiting,
        Played,
    }

    public string suit;
    public int rank;
    public string playerId;
    public CardState state;
    public const float speed = 150.0f;
    private StateMachine stateMachine;
    private Renderer meshRenderer;
    private Color initialColor;
    private string resourceRank;

    public delegate void OnPlayed(Card card);
    public static OnPlayed onPlayed;

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

        string path = "PlayingCards/Resource/Materials/BackColor_Black/Black_PlayingCards_" + this.suit + this.resourceRank + "_00";
        this.meshRenderer.material = Resources.Load(path, typeof(Material)) as Material;
        transform.position = position;
        transform.Rotate(rotateX, rotateY, rotateZ, Space.Self);
        this.initialColor = this.meshRenderer.material.GetColor("_EmissionColor");
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

    public void Highlight()
    {
        this.meshRenderer.material.SetColor("_EmissionColor", Color.yellow);
    }

    public void RemoveHighlight()
    {
        this.meshRenderer.material.SetColor("_EmissionColor", initialColor);
    }

    private int GetSuitSortingRank()
    {
        switch (this.suit)
        {
            case Constants.CardSuits.Heart:
                return 0;
            case Constants.CardSuits.Diamond:
                return 20;
            case Constants.CardSuits.Spade:
                return 40;
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

    IEnumerator MoveToCenterRoutine()
    {
        Vector3 center;
        var verticalPosition = -19.0f + 0.1f * this.stateMachine.NumCardsPlayed();
        switch (this.playerId)
        {
            case Constants.PlayerIds.Player1:
                center = new Vector3(0.0f, verticalPosition, -45.0f);
                break;
            case Constants.PlayerIds.Player2:
                center = new Vector3(-20.0f, verticalPosition, -25.0f);
                break;
            case Constants.PlayerIds.Player3:
                center = new Vector3(0.0f, verticalPosition, -5.0f);
                break;
            case Constants.PlayerIds.Player4:
                center = new Vector3(20.0f, verticalPosition, -25.0f);
                break;
            default:
                throw new Exception("This card has an invalid player id " + this.playerId);
        }

        var rotate = new Vector3(0.0f, UnityEngine.Random.Range(-5.0f, 5.0f), 0.0f);
        while (transform.position != center)
        {
            transform.position = Vector3.MoveTowards(transform.position, center, speed * Time.deltaTime);
            transform.Rotate(rotate * speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        onPlayed(this);
    }
}
