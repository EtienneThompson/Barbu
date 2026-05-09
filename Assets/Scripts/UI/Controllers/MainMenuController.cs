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

        private const float menuSlideDuration = 0.64f;
        private float MenuSlideSpeed => MenuOffscreenShift / menuSlideDuration;
        private const float MenuRightAnchorMinX = 0.9f;
        private const float MenuRightAnchorMaxX = 0.9875f;
        private const float MenuAnchorMinY = 0.1f;
        private const float MenuAnchorMaxY = 0.9f;
        private float MenuOffscreenShift => this.canvasRectTransform.rect.width * (1f - MenuRightAnchorMinX);

        private RectTransform canvasRectTransform;
        private Coroutine moveCoroutine;

        private Vector2 GetInViewPosition() => Vector2.zero;

        private Vector2 GetOutOfViewPosition()
        {
            bool isRight = Settings.MenuSidePreference == Settings.MenuSide.Right;
            return isRight ? new Vector2(MenuOffscreenShift, 0) : new Vector2(-MenuOffscreenShift, 0);
        }

        public void ApplyMenuSide()
        {
            var rt = this.mainMenuContainer.GetComponent<RectTransform>();
            bool isRight = Settings.MenuSidePreference == Settings.MenuSide.Right;
            rt.anchorMin = isRight
                ? new Vector2(MenuRightAnchorMinX, MenuAnchorMinY)
                : new Vector2(1f - MenuRightAnchorMaxX, MenuAnchorMinY);
            rt.anchorMax = isRight
                ? new Vector2(MenuRightAnchorMaxX, MenuAnchorMaxY)
                : new Vector2(1f - MenuRightAnchorMinX, MenuAnchorMaxY);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = GetOutOfViewPosition();
        }

        private void InitializeMenuSide()
        {
            var rt = this.mainMenuContainer.GetComponent<RectTransform>();
            bool isRight = Settings.MenuSidePreference == Settings.MenuSide.Right;
            rt.anchorMin = isRight
                ? new Vector2(MenuRightAnchorMinX, MenuAnchorMinY)
                : new Vector2(1f - MenuRightAnchorMaxX, MenuAnchorMinY);
            rt.anchorMax = isRight
                ? new Vector2(MenuRightAnchorMaxX, MenuAnchorMaxY)
                : new Vector2(1f - MenuRightAnchorMinX, MenuAnchorMaxY);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = GetInViewPosition();
        }

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
            this.canvasRectTransform = this.mainMenuContainer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            this.InitializeMenuSide();

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
                this.StartMoveMenu(this.GetOutOfViewPosition(), MenuSlideSpeed);
            }
            else
            {
                this.telemetryService.LogInfo("Moving menu into view");
                this.singleRoundMenuController.FollowMenuIn();
                this.StartMoveMenu(this.GetInViewPosition(), MenuSlideSpeed);
            }
        }

        public void ShowMenu()
        {
            if (!this.IsUIVisible(this.mainMenuContainer.GetComponent<RectTransform>()))
            {
                this.telemetryService.LogInfo("Moving menu into view");
                this.singleRoundMenuController.FollowMenuIn();
                this.StartMoveMenu(this.GetInViewPosition(), MenuSlideSpeed);
            }
        }

        public void HideMenu()
        {
            if (this.IsUIVisible(this.mainMenuContainer.GetComponent<RectTransform>()))
            {
                this.telemetryService.LogInfo("Moving menu out of view");
                this.singleRoundMenuController.FollowMenuOut();
                this.StartMoveMenu(this.GetOutOfViewPosition(), MenuSlideSpeed);
            }
        }

        private void StartMoveMenu(Vector2 target, float speed)
        {
            if (this.moveCoroutine != null)
                StopCoroutine(this.moveCoroutine);
            this.moveCoroutine = StartCoroutine(this.MoveMenu(target, speed));
        }

        bool IsUIVisible(RectTransform rectTransform)
        {
            if (rectTransform == null) return false;
            return Mathf.Abs(rectTransform.anchoredPosition.x) < MenuOffscreenShift / 2f;
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
