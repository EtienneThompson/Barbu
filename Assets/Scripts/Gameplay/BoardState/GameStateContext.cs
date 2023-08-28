using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext : IEventListener
{
    private readonly StateMachine stateMachine;
    private readonly RoundContext roundContext;
    private IGameState current;
    private bool isPaused;

    public GameStateContext(RoundContext roundContext)
    {
        this.stateMachine = new StateMachine();
        this.roundContext = roundContext;
        this.Setup();
    }

    public void Next()
    {
        // Don't move to the next state if gets called while setting up.
        if (this.stateMachine.IsSettingUp())
        {
            return;
        }

        // If next is called, move to the next state, but only start if the machine
        // isn't paused.
        current.GoNext();

        if (!this.isPaused)
        {
            current.Start();
        }
    }

    public void Start()
    {
        if (this.stateMachine.IsSettingUp())
        {
            return;
        }

        if (!this.isPaused)
        {
            current.Start();
        }
    }

    public void Pause()
    {
        // Setting isPaused to true will break the computer states from running.
        this.isPaused = true;
    }

    public void Resume()
    {
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

    public bool IsCurrentRoundPositive()
    {
        return this.roundContext.IsRoundPositive();
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
