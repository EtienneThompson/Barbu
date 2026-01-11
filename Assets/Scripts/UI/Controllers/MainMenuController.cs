namespace Barbu
{
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class MainMenuController : MonoBehaviour
    {
        private Camera mainCamera;
        private GameObject mainMenuContainer;
        private Button settingsButton;
        private Button gamesButton;
        private Button rulesButton;
        private Button scoreButton;

        private GameObject settingsMenu;
        private GameObject gamesMenu;
        private GameObject howToPlayScreen;
        private GameObject inGamePoints;
        private GameObject roundOverlay;
        private ScoreMenuController scoreMenuController;

        private IStateMachine stateMachine;
        private IEventsController eventsController;
        private GlobalContext globalContext;
        private ITelemetryService telemetryService;

        private Vector2 inViewRelativePosition = new Vector2(-45, 0);
        private Vector2 outOfViewRelativePosition = new Vector2(45, 0);

        [Inject]
        public void Init(
            IStateMachine stateMachine,
            IEventsController eventsController,
            ITelemetryService telemetryService)
        {
            this.stateMachine = stateMachine;
            this.eventsController = eventsController;
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            this.telemetryService.LogInfo("Enabling MainMenuController");

            this.globalContext = GlobalContext.GetInstance();

            var cameraObject = GameObjectExtensions.FindGameObjectByName("Main Camera");
            this.mainCamera = cameraObject.GetComponent<Camera>();

            this.mainMenuContainer = GameObjectExtensions.FindGameObjectByName("MainMenuContainer");

            this.settingsButton = GetButtonObject("SettingsButton");
            this.gamesButton = GetButtonObject("GamesButton");
            this.rulesButton = GetButtonObject("RulesButton");
            this.scoreButton = GetButtonObject("ScoreButton");

            this.settingsMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SettingsMenu, findInactive: true);
            this.gamesMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.GamesMenu, findInactive: true);
            this.howToPlayScreen = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.HowToPlayScreen, findInactive: true);
            this.inGamePoints = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            this.roundOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.RoundOverlay, findInactive: true);
            var scoreCanvas = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.ScoreMenuCanvas, findInactive: true);
            this.scoreMenuController = scoreCanvas.GetComponent<ScoreMenuController>();

            //this.settingsButton.interactable = true;
            this.settingsButton.onClick.AddListener(this.HandleSettingsButtonClick);
            this.gamesButton.onClick.AddListener(this.HandleGamesButtonClick);
            this.rulesButton.onClick.AddListener(this.HandleRulesButtonClick);
            this.scoreButton.onClick.AddListener(this.HandleScoreButtonClick);
        }

        public void OnDisable()
        {
            this.telemetryService.LogInfo("Disabling MainMenuController");
            this.settingsButton.onClick.RemoveAllListeners();
            this.gamesButton.onClick.RemoveAllListeners();
            this.rulesButton.onClick.RemoveAllListeners();
            this.scoreButton.onClick.RemoveAllListeners();
        }

        public void ToggleMenuVisibility()
        {
            if (this.IsUIVisible(this.mainMenuContainer.GetComponent<RectTransform>()))
            {
                this.telemetryService.LogInfo("Moving menu out of view");
                StartCoroutine(this.MoveMenu(this.outOfViewRelativePosition));
            }
            else
            {
                this.telemetryService.LogInfo("Moving menu into view");
                StartCoroutine(this.MoveMenu(this.inViewRelativePosition));
            }
        }

        bool IsUIVisible(RectTransform rectTransform)
        {
            if (rectTransform == null) return false;

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Screen bounds
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Check if any corner is inside the screen
            foreach (Vector3 corner in corners)
            {
                if (corner.x >= 0 && corner.x <= screenWidth &&
                    corner.y >= 0 && corner.y <= screenHeight)
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleSettingsButtonClick()
        {
            this.telemetryService.LogInfo("Settings button clicked");
            this.ToggleMenuVisibility();
            this.settingsMenu.SetActive(true);
        }

        private void HandleGamesButtonClick()
        {
            this.telemetryService.LogInfo("Games button clicked");
            this.ToggleMenuVisibility();
            this.gamesMenu.SetActive(true);
        }

        private void HandleRulesButtonClick()
        {
            this.telemetryService.LogInfo("Rules button clicked");
            this.ToggleMenuVisibility();
            this.howToPlayScreen.SetActive(true);
        }

        private void HandleScoreButtonClick()
        {
            this.telemetryService.LogInfo("Score button click");
            this.ToggleMenuVisibility();
            this.stateMachine.SetMenuOpen(true);
            this.eventsController.Fire(EventNames.PauseGame);
            this.inGamePoints.SetActive(false);
            this.roundOverlay.SetActive(false);
            this.scoreMenuController.DisplayScores(
                this.globalContext.RoundWorkflow.GetPlayerPoints(),
                this.globalContext.RoundWorkflow.GetCurrentRoundIndex(),
                skipAnimations: true);
        }

        private Button GetButtonObject(string name)
        {
            var buttonObject = GameObjectExtensions.FindGameObjectByName(name);
            return buttonObject.GetComponent<Button>();
        }

        private IEnumerator MoveMenu(Vector2 finalPosition)
        {
            var baseMoveSpeed = 300.0f;
            var rectTransform = this.mainMenuContainer.GetComponent<RectTransform>();
            while (rectTransform.anchoredPosition != finalPosition)
            {
                rectTransform.anchoredPosition = Vector2.MoveTowards(
                    rectTransform.anchoredPosition,
                    finalPosition,
                    baseMoveSpeed * Time.deltaTime);
                yield return null;

            }
        }
    }
}
