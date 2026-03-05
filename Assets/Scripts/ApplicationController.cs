namespace Barbu
{
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using UnityEngine;
    using UnityEngine.EventSystems;
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
            scoreBoard?.SetActive(false);
            GameObject gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
            gamesMenu?.SetActive(false);
            GameObject settingsMenu = GameObject.Find(Constants.GameObjects.SettingsMenu);
            settingsMenu?.SetActive(false);
            GameObject singleRoundMenu = GameObject.Find(Constants.GameObjects.SingleRoundMenu);
            singleRoundMenu?.SetActive(false);
            GameObject roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
            roundOverlay?.SetActive(false);

            if (!Settings.HasSeenHowToPlayByDefault())
            {
                GameObject howToPlayScreen = GameObject.Find(Constants.GameObjects.HowToPlayScreen);
                howToPlayScreen.SetActive(true);
                Settings.SetSeenHowToPlayByDefault();
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("UI element clicked");
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    Card card = hit.collider.GetComponent<Card>();
                    if (card != null)
                    {
                        card.OnPointerClick();
                        return;
                    }
                }

                // Default Behavior of the app, if an interaction hits nothing.
                var mainMenu = GameObjectExtensions.FindGameObjectByName("MainMenu");
                var mainMenuController = mainMenu.GetComponent<MainMenuController>();
                mainMenuController.ToggleMenuVisibility();
            }
        }
    }
}
