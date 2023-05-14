using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext
{
    private IGameState current;

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

    public void CleanUp()
    {
        current.CleanUp();
    }

    public void SetState(IGameState newState)
    {
        this.current = newState;
    }

    public IGameState GetCurrentState()
    {
        return this.current;
    }
}
