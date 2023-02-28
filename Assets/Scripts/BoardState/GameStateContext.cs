using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext
{
    private GameState current;

    public GameStateContext()
    {
    }

    public void Next()
    {
        current.GoNext();
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
}
