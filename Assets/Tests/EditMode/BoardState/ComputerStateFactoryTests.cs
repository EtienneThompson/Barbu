namespace Barbu.Tests.EditMode.BoardState
{
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;
    using UnityEngine;

    // Settings.ComputerDifficultyPreference reads/writes the real PlayerPrefs key
    // used by the actual game, so tests must save and restore whatever was there
    // before, rather than leaving the player's difficulty setting altered.
    public class ComputerStateFactoryTests
    {
        private const string DifficultyKey = Constants.PlayerPrefsKeys.ComputerDifficulty;
        private bool keyExistedBefore;
        private int previousValue;

        [SetUp]
        public void SetUp()
        {
            this.keyExistedBefore = PlayerPrefs.HasKey(DifficultyKey);
            this.previousValue = PlayerPrefs.GetInt(DifficultyKey);
        }

        [TearDown]
        public void TearDown()
        {
            if (this.keyExistedBefore)
            {
                PlayerPrefs.SetInt(DifficultyKey, this.previousValue);
            }
            else
            {
                PlayerPrefs.DeleteKey(DifficultyKey);
            }
        }

        [Test]
        public void GetComputerStateFromSettings_Easy_ReturnsEasyComputerState()
        {
            Settings.ComputerDifficultyPreference = Settings.ComputerDifficulty.Easy;
            var factory = new ComputerStateFactory(new FakeStateMachine(), new FakeTelemetryService());

            var state = factory.GetComputerStateFromSettings(new HeartsRound(), "1", new Hand());

            Assert.IsInstanceOf<EasyComputerState>(state);
        }

        [Test]
        public void GetComputerStateFromSettings_Normal_ReturnsNormalComputerState()
        {
            Settings.ComputerDifficultyPreference = Settings.ComputerDifficulty.Normal;
            var factory = new ComputerStateFactory(new FakeStateMachine(), new FakeTelemetryService());

            var state = factory.GetComputerStateFromSettings(new HeartsRound(), "1", new Hand());

            Assert.IsInstanceOf<NormalComputerState>(state);
        }

        [Test]
        public void GetComputerStateFromSettings_Hard_ReturnsHardComputerState()
        {
            Settings.ComputerDifficultyPreference = Settings.ComputerDifficulty.Hard;
            var factory = new ComputerStateFactory(new FakeStateMachine(), new FakeTelemetryService());

            var state = factory.GetComputerStateFromSettings(new HeartsRound(), "1", new Hand());

            Assert.IsInstanceOf<HardComputerState>(state);
        }
    }
}
