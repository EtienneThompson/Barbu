using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private static readonly StateMachine instance = new StateMachine();

    private int numCardsPlayed = 0;
    private bool canCardBePlayed = false;
    private string startingSuit = string.Empty;
    private bool menuOpen = false;

    private bool playerCardMustBeStartingSuit;

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

    public string GetStartingSuit()
    {
        return instance.startingSuit;
    }

    public void SetStartingSuit(string suit)
    {
        instance.startingSuit = suit;
    }

    public void SetPlayerMustPlayStartingSuit(bool isStartingPlayer, Hand hand)
    {
        instance.playerCardMustBeStartingSuit = !isStartingPlayer && hand.CardsInSuit(instance.startingSuit).Count > 0;
    }

    public bool MustPlayCardInStartingSuit()
    {
        return instance.playerCardMustBeStartingSuit;
    }

    public bool IsMenuOpen()
    {
        return instance.menuOpen;
    }

    public void SetMenuOpen(bool state)
    {
        instance.menuOpen = state;
    }
}
