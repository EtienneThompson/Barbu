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
        private Button tradGameButton;
        private Button singleGameButton;
        private Button chaosGameButton;
        private Button rulesButton;
        private Button scoreButton;

        private GameBoard gameBoard;
        private GameObject settingsMenu;
        private GameObject howToPlayScreen;
        private GameObject inGamePoints;
        private GameObject roundOverlay;
        private SingleRoundMenuController singleRoundMenuController;
        private ScoreMenuController scoreMenuController;

        private IStateMachine stateMachine;
        private IEventsController eventsController;
        private GlobalContext globalContext;
        private ITelemetryService telemetryService;

        private Vector2 inViewRelativePosition = new Vector2(-45, 0);
        private Vector2 outOfViewRelativePosition = new Vector2(45, 0);

        private const float menuSlideSpeed = 300f;

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
            this.tradGameButton = GetButtonObject("TradGameButton");
            this.singleGameButton = GetButtonObject("SingleGameButton");
            this.chaosGameButton = GetButtonObject("ChaosGameButton");
            this.rulesButton = GetButtonObject("RulesButton");
            this.scoreButton = GetButtonObject("ScoreButton");

            this.gameBoard = GameObject.Find(Constants.GameObjects.GameBoard).GetComponent<GameBoard>();
            this.settingsMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SettingsMenu, findInactive: true);
            this.howToPlayScreen = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.HowToPlayScreen, findInactive: true);
            this.inGamePoints = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            this.roundOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.RoundOverlay, findInactive: true);
            var singleRoundMenuGO = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SingleRoundMenu, findInactive: true);
            this.singleRoundMenuController = singleRoundMenuGO.GetComponent<SingleRoundMenuController>();
            var scoreCanvas = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.ScoreMenuCanvas, findInactive: true);
            this.scoreMenuController = scoreCanvas.GetComponent<ScoreMenuController>();

            this.settingsButton.onClick.AddListener(this.HandleSettingsButtonClick);
            this.tradGameButton.onClick.AddListener(this.HandleTradGameButtonClick);
            this.singleGameButton.onClick.AddListener(this.HandleSingleGameButtonClick);
            this.chaosGameButton.onClick.AddListener(this.HandleChaosGameButtonClick);
            this.rulesButton.onClick.AddListener(this.HandleRulesButtonClick);
            this.scoreButton.onClick.AddListener(this.HandleScoreButtonClick);
        }

        public void OnDisable()
        {
            this.telemetryService.LogInfo("Disabling MainMenuController");
            this.settingsButton.onClick.RemoveAllListeners();
            this.tradGameButton.onClick.RemoveAllListeners();
            this.singleGameButton.onClick.RemoveAllListeners();
            this.chaosGameButton.onClick.RemoveAllListeners();
            this.rulesButton.onClick.RemoveAllListeners();
            this.scoreButton.onClick.RemoveAllListeners();
        }

        public void ToggleMenuVisibility()
        {
            if (this.IsUIVisible(this.mainMenuContainer.GetComponent<RectTransform>()))
            {
                this.telemetryService.LogInfo("Moving menu out of view");
                this.singleRoundMenuController.FollowMenuOut();
                StartCoroutine(this.MoveMenu(this.outOfViewRelativePosition, menuSlideSpeed));
            }
            else
            {
                this.telemetryService.LogInfo("Moving menu into view");
                this.singleRoundMenuController.FollowMenuIn();
                StartCoroutine(this.MoveMenu(this.inViewRelativePosition, menuSlideSpeed));
            }
        }

        public void ShowMenu()
        {
            if (!this.IsUIVisible(this.mainMenuContainer.GetComponent<RectTransform>()))
            {
                this.telemetryService.LogInfo("Moving menu into view");
                this.singleRoundMenuController.FollowMenuIn();
                StartCoroutine(this.MoveMenu(this.inViewRelativePosition, menuSlideSpeed));
            }
        }

        public void HideMenu()
        {
            if (this.IsUIVisible(this.mainMenuContainer.GetComponent<RectTransform>()))
            {
                this.telemetryService.LogInfo("Moving menu out of view");
                this.singleRoundMenuController.FollowMenuOut();
                StartCoroutine(this.MoveMenu(this.outOfViewRelativePosition, menuSlideSpeed));
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

        private void HandleTradGameButtonClick()
        {
            this.telemetryService.LogInfo("Traditional game button clicked");
            this.HideMenu();
            this.stateMachine.SetMenuOpen(false);
            this.gameBoard.CreateNewGame(Constants.TraditionalRoundManager.GameName, null);
        }

        private void HandleSingleGameButtonClick()
        {
            this.telemetryService.LogInfo("Single game button clicked");
            this.singleRoundMenuController.ToggleMenuVisibility();
        }

        private void HandleChaosGameButtonClick()
        {
            this.telemetryService.LogInfo("Chaos game button clicked");
            this.HideMenu();
            this.stateMachine.SetMenuOpen(false);
            this.gameBoard.CreateNewGame(Constants.ChaosRoundManager.GameName, null);
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

        private IEnumerator MoveMenu(Vector2 finalPosition, float speed)
        {
            var baseMoveSpeed = speed;
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
