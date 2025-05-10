namespace Barbu
{
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using UnityEngine;

    public class ApplicationController : MonoBehaviour
    {
        private ITelemetryService telemetryService;

        void Awake()
        {
            Application.targetFrameRate = 60;

            this.telemetryService = TelemetryService.GetInstance();
            this.telemetryService.LogInfo("Application launched");

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
