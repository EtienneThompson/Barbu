using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private static readonly StateMachine instance = new StateMachine();

    private int numCardsPlayed = 0;

    public StateMachine()
    {
    }

    public void IncrementNumCardsPlayed() {
        instance.numCardsPlayed += 1;
    }

    public int NumCardsPlayed() {
        return instance.numCardsPlayed;
    }
}
