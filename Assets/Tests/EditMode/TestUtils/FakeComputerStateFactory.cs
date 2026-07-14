namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;

    /// <summary>
    /// GameState double that never touches Card visuals (Highlight/RemoveHighlight
    /// dereference a MeshRenderer that only exists after the real
    /// Card.InitializeGameObject, which loads Resources/shaders we don't want in
    /// unit tests), so it's safe to Start()/CleanUp() directly in EditMode tests.
    /// </summary>
    public class RecordingGameState : GameState
    {
        public int StartCallCount { get; private set; }

        public int CleanUpCallCount { get; private set; }

        public RecordingGameState(IStateMachine stateMachine, ITelemetryService telemetryService, IRound round, string id, Hand hand)
            : base(stateMachine, telemetryService, round, hand, id)
        {
        }

        public override void Start()
        {
            this.StartCallCount++;
        }

        public override void CleanUp()
        {
            this.CleanUpCallCount++;
        }
    }

    public class FakeComputerStateFactory : IComputerStateFactory
    {
        public List<string> RequestedPlayerIds { get; } = new();

        public List<RecordingGameState> CreatedStates { get; } = new();

        public GameState GetComputerStateFromSettings(IRound round, string id, Hand hand)
        {
            this.RequestedPlayerIds.Add(id);
            var state = new RecordingGameState(new FakeStateMachine(), new FakeTelemetryService(), round, id, hand);
            this.CreatedStates.Add(state);
            return state;
        }
    }
}
