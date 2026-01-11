namespace Barbu
{
    using Barbu.Core.Telemetry;
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

        private ITelemetryService telemetryService;

        [Inject]
        public void Init(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            this.telemetryService.LogInfo("Enabling MainMenuController");

            var cameraObject = GameObjectExtensions.FindGameObjectByName("Main Camera");
            this.mainCamera = cameraObject.GetComponent<Camera>();

            this.mainMenuContainer = GameObjectExtensions.FindGameObjectByName("MainMenuContainer");

            this.settingsButton = GetButtonObject("SettingsButton");
            this.gamesButton = GetButtonObject("GamesButton");
            this.rulesButton = GetButtonObject("RulesButton");
            this.scoreButton = GetButtonObject("ScoreButton");

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
                var outOfViewRelativePosition = new Vector2(45, 0);
                StartCoroutine(this.MoveMenu(outOfViewRelativePosition));
            }
            else
            {
                this.telemetryService.LogInfo("Moving menu into view");
                var inViewRelativePosition = new Vector2(-45, 0);
                StartCoroutine(this.MoveMenu(inViewRelativePosition));
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
        }

        private void HandleGamesButtonClick()
        {
            this.telemetryService.LogInfo("Games button clicked");
        }

        private void HandleRulesButtonClick()
        {
            this.telemetryService.LogInfo("Rules button clicked");
        }

        private void HandleScoreButtonClick()
        {
            this.telemetryService.LogInfo("Score button click");
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
