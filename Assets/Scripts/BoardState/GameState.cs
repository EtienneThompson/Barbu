using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameState
{
    string PlayerId { get; }

    void Start();
    void CleanUp();
    void GoNext();
    Hand GetHand();
    void SetHand(Hand newHand);
}
