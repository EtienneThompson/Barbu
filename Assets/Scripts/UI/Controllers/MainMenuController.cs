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
        private Transform mainMenuContainer;
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

            this.mainMenuContainer = transform.Find("MainMenuContainer");

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

        public void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (this.IsTransformInCameraView(this.mainMenuContainer, this.mainCamera))
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
        }

        /// <summary>
        /// Checks if a transform's position is within the camera's viewport.
        /// </summary>
        bool IsTransformInCameraView(Transform obj, Camera cam)
        {
            if (obj == null || cam == null)
                return false;

            // Convert world position to viewport space
            Vector3 viewportPos = cam.WorldToViewportPoint(obj.position);

            // Check if object is in front of the camera
            if (viewportPos.z <= 0)
                return false;

            // Check if within viewport bounds
            return viewportPos.x >= 0 && viewportPos.x <= 1 &&
                   viewportPos.y >= 0 && viewportPos.y <= 1;
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
