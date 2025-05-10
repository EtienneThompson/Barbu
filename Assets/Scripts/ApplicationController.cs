namespace Barbu
{
    using Barbu.Core.Telemetry;
    using UnityEngine;
    using Zenject;

    public class ApplicationController : MonoBehaviour
    {
        private ITelemetryService telemetryService;

        [Inject]
        public void Init(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService;
        }

        void Awake()
        {
            this.telemetryService.LogInfo("Application launched");
            Application.targetFrameRate = 60;

            // Hide any UI objects.
            GameObject scoreBoard = GameObject.Find(Constants.GameObjects.ScoreMenuCanvas);
            scoreBoard.SetActive(false);
            GameObject gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
            gamesMenu.SetActive(false);
            GameObject settingsMenu = GameObject.Find(Constants.GameObjects.SettingsMenu);
            settingsMenu.SetActive(false);
            GameObject singleRoundMenu = GameObject.Find(Constants.GameObjects.SingleRoundMenu);
            singleRoundMenu.SetActive(false);

            if (!Settings.HasSeenHowToPlayByDefault())
            {
                GameObject howToPlayScreen = GameObject.Find(Constants.GameObjects.HowToPlayScreen);
                howToPlayScreen.SetActive(true);
                Settings.SetSeenHowToPlayByDefault();
            }
        }
    }
}
