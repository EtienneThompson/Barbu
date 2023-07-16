using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// EventsController is the main hub at which different systems in the game can
/// subscribe to receive events from other systems.
///
/// For example, the gameplay loop system can subscribe to the pause or resume
/// delegates to be told when to stop by another system, i.e. the menuing system.
///
/// The singleton state machine should be the only controller to call these methods.
/// In practice, this should also be a singleton.
/// </summary>
public class EventsController
{
    public delegate void PauseGame();
    public static PauseGame pauseGame;

    public delegate void ResumeGame();
    public static ResumeGame resumeGame;

    private static EventsController singleton;

    private EventsController()
    {

    }

    public static EventsController GetInstance()
    {
        if (singleton == null)
        {
            singleton = new EventsController();
        }

        return singleton;
    }

    public void Pause()
    {
        Debug.Log("EventsController - Pausing game");
        pauseGame();
    }

    public void Resume()
    {
        Debug.Log("EventsController - Resuming game");
        resumeGame();
    }
}
