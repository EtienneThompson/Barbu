using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext : IEventListener
{
    private StateMachine stateMachine;
    private IGameState current;
    private bool isPaused;

    public GameStateContext()
    {
        this.stateMachine = new StateMachine();
        this.Setup();
    }

    public void Next()
    {
        // If next is called, move to the next state, but only start if the machine
        // isn't paused.
        Debug.Log("Going to next state");
        current.GoNext();

        if (!this.isPaused)
        {
            Debug.Log("Game is not paused, starting...");
            current.Start();
        }
    }

    public void Start()
    {
        Debug.Log("Starting game context");
        if (!this.isPaused)
        {
            Debug.Log("Game is not paused, starting...");
            current.Start();
        }
    }

    public void Pause()
    {
        Debug.Log("Pause event received. Pausing game...");
        // Setting isPaused to true will break the computer states from running.
        this.isPaused = true;
    }

    public void Resume()
    {
        Debug.Log("Resume event received. Resuming game...");
        // Need to start the state machine again, so will call start.
        this.isPaused = false;
        this.Start();
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

    public void Setup()
    {
        // Listen to global pause/resume events.
        EventsController.pauseGame += this.Pause;
        EventsController.resumeGame += this.Resume;
    }

    public void Destroy()
    {
        // Stop listening.
        EventsController.pauseGame -= this.Pause;
        EventsController.resumeGame -= this.Resume;
    }
}
