using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private static readonly StateMachine instance = new StateMachine();

    private int numCardsPlayed = 0;
    private bool canCardBePlayed = false;

    public StateMachine()
    {
    }

    public int NumCardsPlayed()
    {
        return instance.numCardsPlayed;
    }

    public bool IsCardPlayable()
    {
        return instance.canCardBePlayed;
    }

    public void IncrementNumCardsPlayed()
    {
        instance.numCardsPlayed += 1;
    }

    public void ResetNumCardsPlayed()
    {
        instance.numCardsPlayed = 0;
    }

    public void SetCardPlayable(bool state)
    {
        instance.canCardBePlayed = state;
    }
}
