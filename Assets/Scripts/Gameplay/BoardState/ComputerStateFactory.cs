namespace Barbu.Gameplay.BoardState
{
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay.Rounds;

    public class ComputerStateFactory : IComputerStateFactory
    {
        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;

        public ComputerStateFactory(
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public GameState GetComputerStateFromSettings(
            IRound round,
            string id,
            Hand hand)
        {
            switch (Settings.ComputerDifficultyPreference)
            {
                case Settings.ComputerDifficulty.Hard:
                    return new HardComputerState(this.stateMachine, this.telemetryService, round, id, hand);
                case Settings.ComputerDifficulty.Normal:
                    return new NormalComputerState(this.stateMachine, this.telemetryService, round, id, hand);
                case Settings.ComputerDifficulty.Easy:
                default:
                    return new EasyComputerState(this.stateMachine, this.telemetryService, round, id, hand);
            }
        }
    }
}
