namespace Barbu.UI.Controllers
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Core.Features;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Components;
    using Barbu.UI.SettingsMenu;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    /// <summary>
    /// Builds the settings menu from <see cref="SettingsRegistry"/> when it opens, one row
    /// prefab per setting.
    /// </summary>
    /// <remarks>
    /// The menu's own furniture (panel, header, scroll view, close button) is authored in the
    /// scene; only the rows inside the scroll view are generated. Adding a setting is a change
    /// to the registry alone.
    ///
    /// The scene wiring below is built by Barbu &gt; Settings Menu &gt; Rebuild Menu Hierarchy.
    /// </remarks>
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("Scene")]
        [Tooltip("The row area's layout group. Its left and right padding is set from the safe area.")]
        [SerializeField]
        private LayoutGroup safeAreaPadding;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform contentRoot;

        [SerializeField]
        private Button closeButton;

        [Tooltip("Disabled sample rows kept in the scene so the menu can be seen without entering play mode.")]
        [SerializeField]
        private GameObject[] previewRows;

        [Header("Row prefabs")]
        [SerializeField]
        private SelectorSettingRowView selectorRowPrefab;

        [SerializeField]
        private ToggleSettingRowView toggleRowPrefab;

        [Header("Feature flags (development builds only)")]
        [SerializeField]
        private GameObject featureFlagHeader;

        [Tooltip("Shown in place of the flag rows while no flag is registered in FeatureRegistry.")]
        [SerializeField]
        private GameObject noFeatureFlagsLabel;

        [SerializeField]
        private Button resetOverridesButton;

        private readonly List<SettingRowView> rows = new List<SettingRowView>();

        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;
        private IFeatureService featureService;
        private bool rebuildRequested;

        [Inject]
        public void Init(IStateMachine stateMachine, ITelemetryService telemetryService, IFeatureService featureService)
        {
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
            this.featureService = featureService;
        }

        public void OnEnable()
        {
            this.telemetryService.LogInfo("Enabling SettingsMenuController");
            this.stateMachine.SetMenuOpen(true);
            this.ApplySafeArea();

            if (this.closeButton != null)
            {
                this.closeButton.onClick.AddListener(this.HandleCloseButtonClick);
            }

            if (this.resetOverridesButton != null)
            {
                this.resetOverridesButton.onClick.AddListener(this.HandleResetOverridesButtonClick);
            }

            this.Rebuild();
        }

        public void OnDisable()
        {
            this.telemetryService.LogInfo("Disabling SettingsMenuController");
            this.stateMachine.SetMenuOpen(false);

            if (this.closeButton != null)
            {
                this.closeButton.onClick.RemoveListener(this.HandleCloseButtonClick);
            }

            if (this.resetOverridesButton != null)
            {
                this.resetOverridesButton.onClick.RemoveListener(this.HandleResetOverridesButtonClick);
            }

            this.ClearRows();
            this.rebuildRequested = false;
        }

        /// <summary>
        /// Rebuilds outside any button callback. A row that asks for a rebuild is asking to be
        /// destroyed, which is not something to do while its own click handler is running.
        /// </summary>
        public void LateUpdate()
        {
            if (!this.rebuildRequested)
            {
                return;
            }

            this.rebuildRequested = false;
            this.Rebuild();
        }

        private void Rebuild()
        {
            var scrollPosition = this.scrollRect != null ? this.scrollRect.verticalNormalizedPosition : 1f;

            this.ClearRows();

            // The samples exist for the scene view; the real rows replace them at runtime.
            if (this.previewRows != null)
            {
                foreach (var previewRow in this.previewRows)
                {
                    if (previewRow != null)
                    {
                        previewRow.SetActive(false);
                    }
                }
            }

            foreach (var definition in SettingsRegistry.All)
            {
                if (this.IsVisible(definition))
                {
                    this.CreateRow(definition);
                }
            }

            this.BuildFeatureFlagSection();

            // The offset reads back as NaN while the content is shorter than the viewport,
            // which is the normal case for a short settings list.
            if (this.scrollRect != null && !float.IsNaN(scrollPosition))
            {
                // Restoring the scroll offset only means anything once the new rows have been
                // laid out and the content has its final height.
                Canvas.ForceUpdateCanvases();
                this.scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollPosition);
            }
        }

        private bool IsVisible(SettingDefinition definition)
        {
            if (definition.DevelopmentOnly && !IsDevelopmentBuild)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.RequiredFeature))
            {
                return true;
            }

            return this.featureService.IsEnabled(definition.RequiredFeature);
        }

        private void CreateRow(SettingDefinition definition)
        {
            var prefab = definition.ControlKind == SettingControlKind.Toggle
                ? (SettingRowView)this.toggleRowPrefab
                : this.selectorRowPrefab;

            if (prefab == null)
            {
                this.telemetryService.LogError(
                    $"No row prefab is assigned for {definition.ControlKind}, so the '{definition.Label}' setting cannot be shown.");
                return;
            }

            var row = Instantiate(prefab, this.contentRoot);
            row.gameObject.name = $"{definition.Label} Row";
            row.gameObject.SetActive(true);
            row.Changed += this.HandleRowChanged;
            row.Bind(definition);
            this.rows.Add(row);
        }

        private void ClearRows()
        {
            foreach (var row in this.rows)
            {
                if (row == null)
                {
                    continue;
                }

                row.Changed -= this.HandleRowChanged;

                // Destroy only takes effect at the end of the frame, so unparent first;
                // otherwise the outgoing rows would still be laying out alongside the new ones.
                row.transform.SetParent(null, worldPositionStays: false);
                Destroy(row.gameObject);
            }

            this.rows.Clear();
        }

        private void HandleRowChanged(SettingRowView row)
        {
            this.telemetryService.LogInfo($"Setting '{row.Definition.Label}' changed to '{row.Definition.DisplayValue}'");

            if (row.Definition.AffectsMenuLayout)
            {
                this.rebuildRequested = true;
            }
        }

        /// <summary>
        /// Insets the rows so a notch cannot cover them, as the left and right spacers in the
        /// old layout did.
        /// </summary>
        /// <remarks>
        /// Only the rows are inset, not the whole menu: the white background and the green
        /// header still run to the edges of the screen. Padding is used rather than anchors
        /// because this object's rect is driven by the layout group above it.
        /// </remarks>
        private void ApplySafeArea()
        {
            if (this.safeAreaPadding == null || Screen.width == 0)
            {
                return;
            }

            var canvas = this.GetComponent<Canvas>();
            var scale = canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;

            var safeArea = Screen.safeArea;
            var left = Mathf.Max(0, Mathf.RoundToInt(safeArea.xMin / scale));
            var right = Mathf.Max(0, Mathf.RoundToInt((Screen.width - safeArea.xMax) / scale));

            var current = this.safeAreaPadding.padding;
            if (current.left == left && current.right == right)
            {
                return;
            }

            // A fresh RectOffset rather than mutating the current one, which the layout group
            // holds by reference and so would not notice changing.
            this.safeAreaPadding.padding = new RectOffset(left, right, current.top, current.bottom);
        }

        private void HandleCloseButtonClick()
        {
            this.telemetryService.LogInfo("Settings close button clicked");
            this.gameObject.SetActive(false);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const bool IsDevelopmentBuild = true;

        private void BuildFeatureFlagSection()
        {
            var definitions = FeatureFlagSettings.CreateDefinitions(this.featureService);

            this.SetFeatureFlagSectionActive(true);

            // The header is authored in the scene rather than generated, so it has to be moved
            // below the settings rows that were just added before the flag rows follow it.
            this.featureFlagHeader.transform.SetAsLastSibling();

            // An empty list is still an answer to "which flags are there?". Hiding the section
            // instead makes a working panel look like a broken one.
            if (definitions.Count == 0)
            {
                if (this.noFeatureFlagsLabel != null)
                {
                    this.noFeatureFlagsLabel.SetActive(true);
                    this.noFeatureFlagsLabel.transform.SetAsLastSibling();
                }

                return;
            }

            if (this.noFeatureFlagsLabel != null)
            {
                this.noFeatureFlagsLabel.SetActive(false);
            }

            foreach (var definition in definitions)
            {
                this.CreateRow(definition);
            }
        }

        private void HandleResetOverridesButtonClick()
        {
            this.telemetryService.LogInfo("Feature flag overrides reset");
            this.featureService.ClearAllOverrides();
            this.rebuildRequested = true;
        }
#else
        private const bool IsDevelopmentBuild = false;

        private void BuildFeatureFlagSection()
        {
            this.SetFeatureFlagSectionActive(false);
        }

        private void HandleResetOverridesButtonClick()
        {
        }
#endif

        private void SetFeatureFlagSectionActive(bool active)
        {
            if (this.featureFlagHeader != null)
            {
                this.featureFlagHeader.SetActive(active);
            }

            if (this.resetOverridesButton != null)
            {
                this.resetOverridesButton.gameObject.SetActive(active);
            }

            if (!active && this.noFeatureFlagsLabel != null)
            {
                this.noFeatureFlagsLabel.SetActive(false);
            }
        }
    }
}
