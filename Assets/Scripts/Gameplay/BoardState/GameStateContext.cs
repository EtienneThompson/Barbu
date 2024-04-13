namespace Barbu.Gameplay.BoardState
{
    using Barbu.Core;
    using Barbu.Gameplay.Rounds;
    using Barbu.Interfaces;
    using Barbu.Interfaces.BoardState;
    using Barbu.Models;

    public class GameStateContext : IEventListener
    {
        private readonly StateMachine stateMachine;
        private readonly EventsController eventsController;
        private readonly RoundContext roundContext;
        private IGameState current;
        private bool isPaused;

        public GameStateContext(RoundContext roundContext)
        {
            this.stateMachine = new StateMachine();
            this.eventsController = EventsController.GetInstance();
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

        public bool IsPointEarningCard(string cardName)
        {
            return this.roundContext.IsPointEarningCard(cardName);
        }

        public int GetCardPointValue(string cardName)
        {
            return this.roundContext.GetCardPointValue(cardName);
        }

        public void Setup()
        {
            // Listen to global pause/resume events.
            this.eventsController.Subscribe(EventNames.PauseGame, this.Pause);
            this.eventsController.Subscribe(EventNames.ResumeGame, this.Resume);
        }

        public void Destroy()
        {
            // Stop listening.
            this.eventsController.Unsubscribe(EventNames.PauseGame, this.Pause);
            this.eventsController.Unsubscribe(EventNames.ResumeGame, this.Resume);
        }
    }
}