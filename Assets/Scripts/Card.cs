using System.Collections;
using System.Collections.Generic;
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

    public delegate void OnPlayed(Card card);
    public static OnPlayed onPlayed;

    public void Initialize(string s, string r, string playerId, float rotateX, float rotateY, float rotateZ)
    {
        this.meshRenderer = GetComponent<MeshRenderer>();
        this.suit = s;

        this.rank = System.Int32.Parse(r);
        if (this.rank == 1)
        {
            // Handle case where Ace is lowest in the resource pack, but highest rank.
            this.rank = 14;
        }

        this.playerId = playerId;
        this.state = CardState.Waiting;
        this.stateMachine = new StateMachine();

        string path = "PlayingCards/Resource/Materials/BackColor_Black/Black_PlayingCards_" + this.suit + r + "_00";
        this.meshRenderer.material = Resources.Load(path, typeof(Material)) as Material;
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
        Debug.Log("RemoveHighlight");
        Debug.Log(this.GetName());
        this.meshRenderer.material.SetColor("_EmissionColor", initialColor);
    }

    IEnumerator MoveToCenterRoutine()
    {
        var center = new Vector3(0.0f, -19.0f + 0.1f * stateMachine.NumCardsPlayed(), -25.0f);
        var rotate = new Vector3(0.0f, Random.Range(-5.0f, 5.0f), 0.0f);
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
