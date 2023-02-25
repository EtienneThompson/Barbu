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
    public string rank;
    public bool inPlayerHand;
    public CardState state;
    public const float speed = 150.0f;
    private StateMachine stateMachine;

    public delegate void OnPlayed(Card card);
    public static OnPlayed onPlayed;

    public void Initialize(string s, string r, bool inHand) {
        this.suit = s;
        this.rank = r;
        this.inPlayerHand = inHand;
        this.state = CardState.Waiting;
        this.stateMachine = new StateMachine();

        string path = "PlayingCards/Resource/Materials/BackColor_Black/Black_PlayingCards_" + this.suit + this.rank + "_00";
        GetComponent<MeshRenderer>().material = Resources.Load(path, typeof(Material)) as Material;
        transform.Rotate(-45.0f, 0.0f, 0.0f, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state == CardState.Waiting && 
            this.stateMachine.IsCardPlayable() && 
            Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    stateMachine.IncrementNumCardsPlayed();
                    StartCoroutine(MoveToCenterRoutine());
                    this.state = CardState.Played;
                    this.stateMachine.SetCardPlayable(false);
                }
            }
        }
    }

    IEnumerator MoveToCenterRoutine()
    {
        var center = new Vector3(0.0f, 1.0f + 0.1f * stateMachine.NumCardsPlayed(), 0.0f);
        var rotate = new Vector3(0.0f, Random.Range(-5.0f, 5.0f), 0.0f);
        while (transform.position != center) {
            transform.position = Vector3.MoveTowards(transform.position, center, speed * Time.deltaTime);
            transform.Rotate(rotate * speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        onPlayed(this);
    }
}
