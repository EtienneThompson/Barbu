using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext
{
    private GameState current;
    private string startingSuit;

    public GameStateContext()
    {
    }

    public void Next()
    {
        current.GoNext();
        current.Start();
    }

    public void Start()
    {
        current.Start();
    }

    public void SetState(GameState newState)
    {
        this.current = newState;
    }

    public GameState GetCurrentState()
    {
        return this.current;
    }

        public void SetStartingSuit(string suit)
    {
        this.startingSuit = suit;
    }

    public string GetStartingSuit()
    {
        return this.startingSuit;
    }
}
