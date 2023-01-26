using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private string suit;
    private string rank;
    private bool inPlayerHand;

    public void Initialize(string suit, string rank, bool inHand, Vector3 position) {
        Debug.Log("Initialize");
        this.suit = suit;
        this.rank = rank;
        this.inPlayerHand = inHand;

        GameObject card = Instantiate(Resources.Load("BlankPlayingCard", typeof(GameObject)), position, Quaternion.identity) as GameObject;

        string path = "PlayingCards/Resource/Materials/BackColor_Black/Black_PlayingCards_" + this.suit + this.rank + "_00";
        Debug.Log(path);
        card.GetComponent<MeshRenderer>().material = Resources.Load(path, typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
