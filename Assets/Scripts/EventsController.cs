using UnityEngine;


/// <summary>
/// EventsController is the main hub at which different systems in the game can
/// subscribe to receive events from other systems.
///
/// For example, the gameplay loop system can subscribe to the pause or resume
/// delegates to be told when to stop by another system, i.e. the menuing system.
/// </summary>
public class EventsController
{
    public delegate void PauseGame();
    public static PauseGame pauseGame;

    public delegate void ResumeGame();
    public static ResumeGame resumeGame;

    public delegate void PlayCard(Card card);
    public static PlayCard playCard;

    public delegate void RoundOver();
    public static RoundOver endRound;

    public delegate void RoundAnimationOver();
    public static RoundAnimationOver roundAnimationOver;

    public delegate void PileResolutionFinished();
    public static PileResolutionFinished endPileResolution;

    private static EventsController singleton;

    private int numCardsFinishedResolvingInPile;

    private EventsController()
    {
        this.numCardsFinishedResolvingInPile = 0;
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

    public void Play(Card card)
    {
        playCard(card);
    }

    public void EndRound()
    {
        endRound();
    }

    public void FinishRoundAnimation()
    {
        roundAnimationOver();
    }

    public void MarkCardAsFinishedResolving()
    {
        this.numCardsFinishedResolvingInPile++;
        if (this.numCardsFinishedResolvingInPile == 4)
        {
            this.numCardsFinishedResolvingInPile = 0;
            endPileResolution();
        }
    }
}
