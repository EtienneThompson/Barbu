using System;

public class GameState : IGameState
{
    protected GameStateContext context;
    protected GameState nextState;
    protected Hand hand;
    protected StateMachine stateMachine;
    public string PlayerId { get; private set; }

    public GameState(GameStateContext context, Hand hand, string id)
    {
        this.context = context;
        this.hand = hand;
        this.PlayerId = id;
        this.stateMachine = new StateMachine();
    }

    public GameState(GameStateContext context, GameState next, Hand hand, string id)
    : this(context, hand, id)
    {
        this.nextState = next;
    }

    public virtual void Start()
    {
        // Do nothing, this will be overridden by states appropriately.
    }

    public virtual void CleanUp()
    {
        // Always adjust the position of the cards in the hand.
        int index = 0;
        var availableCards = this.hand.GetAvailableCards();
        var cardsInHand = availableCards.Count;
        foreach (var card in availableCards)
        {
            card.StartPositionAdjustment(index, cardsInHand);
            index++;
        }
    }

    public virtual void GoNext()
    {
        if (this.nextState == null)
        {
            throw new Exception("No next state set.");
        }

        this.context.SetState(this.nextState);
    }

    public void SetNextState(GameState next)
    {
        this.nextState = next;
    }

    public void SetHand(Hand newHand)
    {
        this.hand = newHand;
    }
}
