namespace Barbu.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Core.Features;
    using Barbu.Core.Telemetry;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Zenject;

    public class SettingsMenuController : MonoBehaviour
    {
        private Dictionary<string, string> HandSortingStrings = new Dictionary<string, string>
        {
            [Settings.SortingOptions.None.ToString()] = "No Sorting",
            [Settings.SortingOptions.LowToHigh.ToString()] = "2 → A",
            [Settings.SortingOptions.HighToLow.ToString()] = "A → 2",
            [Settings.SortingOptions.SuitLowToHigh.ToString()] = "♥ ♦ ♠ ♣ 2 → A",
            [Settings.SortingOptions.SuitHighToLow.ToString()] = "♥ ♦ ♠ ♣ A → 2",
            [Settings.SortingOptions.SuitLowToHighAlternating.ToString()] = "♥ ♠ ♦ ♣ 2 → A",
            [Settings.SortingOptions.SuitHighToLowAlternating.ToString()] = "♥ ♠ ♦ ♣ A → 2",
        };

        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private IFeatureService featureService;
#endif
        private GameObject settingsMenu;
        private Button sortingPreviousBtn;
        private Button sortingNextBtn;
        private Button difficultyPreviousBtn;
        private Button difficultyNextBtn;
        private Button backColorPreviousBtn;
        private Button backColorNextBtn;
        private Button menuSidePreviousBtn;
        private Button menuSideNextBtn;
        private Button closeBtn;
        private Label currentlySelectedSortingOption;
        private Label currentlySelectedDifficulty;
        private Label currentlySelectedBackColor;
        private Label currentlySelectedMenuSide;
        private int currentSortingOption;
        private int currentDifficultyOption;
        private int currentBackColorOption;
        private int currentMenuSideOption;

        // featureService is only stored in builds that can show the flag panel, but the
        // signature stays the same in every build so Zenject has one thing to satisfy.
        [Inject]
        public void Init(IStateMachine stateMachine, ITelemetryService telemetryService, IFeatureService featureService)
        {
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            this.featureService = featureService;
#endif
        }

        public void OnEnable()
        {
            this.stateMachine.SetMenuOpen(true);
            this.settingsMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SettingsMenu, findInactive: true);
            var document = this.settingsMenu.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            var safeArea = Screen.safeArea;
            float notchPct = safeArea.xMin / Screen.width * 100f;
            var leftSpacer = root.Q<VisualElement>("left-notch-spacer");
            var rightSpacer = root.Q<VisualElement>("right-notch-spacer");
            if (notchPct > 0f)
            {
                bool notchOnLeft = Screen.orientation == ScreenOrientation.LandscapeLeft;
                leftSpacer.style.width = notchOnLeft ? Length.Percent(notchPct) : new Length(0);
                rightSpacer.style.width = notchOnLeft ? new Length(0) : Length.Percent(notchPct);
            }
            else
            {
                leftSpacer.style.width = new Length(0);
                rightSpacer.style.width = new Length(0);
            }

            this.sortingPreviousBtn = root.Q<Button>("sortingPrevious");
            this.sortingNextBtn = root.Q<Button>("sortingNext");
            this.difficultyPreviousBtn = root.Q<Button>("difficultyPrevious");
            this.difficultyNextBtn = root.Q<Button>("difficultyNext");
            this.backColorPreviousBtn = root.Q<Button>("backColorPrevious");
            this.backColorNextBtn = root.Q<Button>("backColorNext");
            this.menuSidePreviousBtn = root.Q<Button>("menuSidePrevious");
            this.menuSideNextBtn = root.Q<Button>("menuSideNext");
            this.closeBtn = root.Q<Button>("close");

            this.currentlySelectedSortingOption = root.Q<Label>("currentlySelectedSortingOption");
            this.currentlySelectedDifficulty = root.Q<Label>("currentlySelectedDifficulty");
            this.currentlySelectedBackColor = root.Q<Label>("currentlySelectedBackColor");
            this.currentlySelectedMenuSide = root.Q<Label>("currentlySelectedMenuSide");
            _ = HandSortingStrings.TryGetValue(Settings.SortingPreference.ToString(), out var initialSortingOptionString);
            this.currentlySelectedSortingOption.text = initialSortingOptionString;
            this.currentlySelectedSortingOption.style.unityFontDefinition = FontDefinition.FromFont(Resources.Load<Font>("Fonts/LucidaSansUnicodeRegular"));
            this.currentSortingOption = Array.IndexOf(Settings.HandSortingOptions, Settings.SortingPreference);
            this.currentlySelectedDifficulty.text = Settings.ComputerDifficultyPreference.ToString();
            this.currentDifficultyOption = Array.IndexOf(Settings.ComputerDifficulties, Settings.ComputerDifficultyPreference);
            this.currentBackColorOption = Array.IndexOf(Settings.BackColors, Settings.BackColorPreference);
            this.currentlySelectedBackColor.text = Settings.BackColors[this.currentBackColorOption].ToString();
            this.currentMenuSideOption = Array.IndexOf(Settings.MenuSides, Settings.MenuSidePreference);
            this.currentlySelectedMenuSide.text = Settings.MenuSides[this.currentMenuSideOption].ToString();

            this.sortingPreviousBtn.RegisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
            this.sortingNextBtn.RegisterCallback<ClickEvent>(HandleSortingNextButtonClick);
            this.difficultyPreviousBtn.RegisterCallback<ClickEvent>(HandleDifficultyPreviousButtonClick);
            this.difficultyNextBtn.RegisterCallback<ClickEvent>(HandleDifficultyNextButtonClick);
            this.backColorPreviousBtn.RegisterCallback<ClickEvent>(HandleBackColorPreviousButtonClick);
            this.backColorNextBtn.RegisterCallback<ClickEvent>(HandleBackColorNextButtonClick);
            this.menuSidePreviousBtn.RegisterCallback<ClickEvent>(HandleMenuSidePreviousButtonClick);
            this.menuSideNextBtn.RegisterCallback<ClickEvent>(HandleMenuSideNextButtonClick);
            this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            this.BuildFeatureFlagSection(root.Q<VisualElement>("settings-column"));
#endif
        }

        public void OnDisable()
        {
            this.stateMachine.SetMenuOpen(false);
            this.sortingPreviousBtn.UnregisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
            this.sortingNextBtn.UnregisterCallback<ClickEvent>(HandleSortingNextButtonClick);
            this.difficultyPreviousBtn.UnregisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
            this.difficultyNextBtn.UnregisterCallback<ClickEvent>(HandleDifficultyNextButtonClick);
            this.backColorPreviousBtn.UnregisterCallback<ClickEvent>(HandleBackColorPreviousButtonClick);
            this.backColorNextBtn.UnregisterCallback<ClickEvent>(HandleBackColorNextButtonClick);
            this.menuSidePreviousBtn.UnregisterCallback<ClickEvent>(HandleMenuSidePreviousButtonClick);
            this.menuSideNextBtn.UnregisterCallback<ClickEvent>(HandleMenuSideNextButtonClick);
            this.closeBtn.UnregisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        private void HandleSortingPreviousButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Sorting Previous button clicked");
            this.currentSortingOption = this.SafeMod(this.currentSortingOption - 1, Settings.HandSortingOptions.Length);
            _ = HandSortingStrings.TryGetValue(Settings.HandSortingOptions[this.currentSortingOption].ToString(), out var sortingOptionText);
            this.currentlySelectedSortingOption.text = sortingOptionText;
            Settings.SortingPreference = Settings.HandSortingOptions[this.currentSortingOption];
        }

        private void HandleSortingNextButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Sorting Next button clicked");
            this.currentSortingOption = this.SafeMod(this.currentSortingOption + 1, Settings.HandSortingOptions.Length);
            _ = HandSortingStrings.TryGetValue(Settings.HandSortingOptions[this.currentSortingOption].ToString(), out var sortingOptionText);
            this.currentlySelectedSortingOption.text = sortingOptionText;
            Settings.SortingPreference = Settings.HandSortingOptions[this.currentSortingOption];
        }

        private void HandleDifficultyPreviousButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Difficulty Previous button clicked");
            this.currentDifficultyOption = this.SafeMod(this.currentDifficultyOption - 1, Settings.ComputerDifficulties.Length);
            this.currentlySelectedDifficulty.text = Settings.ComputerDifficulties[this.currentDifficultyOption].ToString();
            Settings.ComputerDifficultyPreference = Settings.ComputerDifficulties[this.currentDifficultyOption];
        }

        private void HandleDifficultyNextButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Difficulty Next Button clicked");
            this.currentDifficultyOption = this.SafeMod(this.currentDifficultyOption + 1, Settings.ComputerDifficulties.Length);
            this.currentlySelectedDifficulty.text = Settings.ComputerDifficulties[this.currentDifficultyOption].ToString();
            Settings.ComputerDifficultyPreference = Settings.ComputerDifficulties[this.currentDifficultyOption];
        }

        private void HandleBackColorPreviousButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Back Color Previous Button clicked");
            this.currentBackColorOption = this.SafeMod(this.currentBackColorOption - 1, Settings.BackColors.Length);
            this.currentlySelectedBackColor.text = Settings.BackColors[this.currentBackColorOption].ToString();
            Settings.BackColorPreference = Settings.BackColors[this.currentBackColorOption];
        }

        private void HandleBackColorNextButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Back Color Next Button clicked");
            this.currentBackColorOption = this.SafeMod(this.currentBackColorOption + 1, Settings.BackColors.Length);
            this.currentlySelectedBackColor.text = Settings.BackColors[this.currentBackColorOption].ToString();
            Settings.BackColorPreference = Settings.BackColors[this.currentBackColorOption];
        }

        private void HandleMenuSidePreviousButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Menu Side Previous button clicked");
            this.currentMenuSideOption = this.SafeMod(this.currentMenuSideOption - 1, Settings.MenuSides.Length);
            this.currentlySelectedMenuSide.text = Settings.MenuSides[this.currentMenuSideOption].ToString();
            Settings.MenuSidePreference = Settings.MenuSides[this.currentMenuSideOption];
            this.NotifyMenuControllersOfSideChange();
        }

        private void HandleMenuSideNextButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Menu Side Next button clicked");
            this.currentMenuSideOption = this.SafeMod(this.currentMenuSideOption + 1, Settings.MenuSides.Length);
            this.currentlySelectedMenuSide.text = Settings.MenuSides[this.currentMenuSideOption].ToString();
            Settings.MenuSidePreference = Settings.MenuSides[this.currentMenuSideOption];
            this.NotifyMenuControllersOfSideChange();
        }

        private void NotifyMenuControllersOfSideChange()
        {
            var mainMenuGO = GameObjectExtensions.FindGameObjectByName("MainMenu");
            mainMenuGO.GetComponent<MainMenuController>().ApplyMenuSide();

            var singleRoundMenuGO = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SingleRoundMenu, findInactive: true);
            singleRoundMenuGO.GetComponent<SingleRoundMenuController>().ApplyMenuSide();
        }

        private void HandleCloseButtonClick(ClickEvent evt)
        {
            this.settingsMenu.SetActive(false);
        }

        private int SafeMod(int numerator, int modulus)
        {
            int result = numerator % modulus;
            return result < 0 ? result + modulus : result;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const string FeatureFlagSectionName = "feature-flag-section";

        /// <summary>
        /// Appends a live feature flag list to the bottom of the settings menu, so flags can
        /// be toggled on a device without a rebuild.
        /// </summary>
        /// <remarks>
        /// Compiled only into the editor and development builds, which is why it is built in
        /// C# rather than authored in SettingsMenu.uxml: UXML has no preprocessor, and the
        /// rows have to be generated from the registry anyway since the flag list changes.
        /// Element names match the ones in SettingsMenu.uss so the generated rows pick up the
        /// same styling as the hand authored ones.
        /// </remarks>
        private void BuildFeatureFlagSection(VisualElement column)
        {
            // The UIDocument keeps its visual tree while the menu GameObject is disabled, so
            // OnEnable would otherwise stack up another copy of this section each reopen.
            column.Q<VisualElement>(FeatureFlagSectionName)?.RemoveFromHierarchy();

            var section = new VisualElement { name = FeatureFlagSectionName };
            section.style.width = Length.Percent(100);
            section.style.flexGrow = 1;
            column.Add(section);

            var header = new Label("Feature Flags");
            header.AddToClassList("header");
            section.Add(header);

            if (this.featureService.Definitions.Count == 0)
            {
                section.Add(new Label("No feature flags defined."));
                return;
            }

            // The flag list has no fixed length and the menu is a phone sized screen, so let
            // the rows scroll instead of pushing the Close button off the bottom.
            var scrollView = new ScrollView();
            scrollView.style.width = Length.Percent(100);
            scrollView.style.flexGrow = 1;
            section.Add(scrollView);

            foreach (var definition in this.featureService.Definitions)
            {
                scrollView.Add(this.BuildFeatureFlagRow(definition.Name));
            }

            var resetButton = new Button(() =>
            {
                this.telemetryService.LogInfo("Feature flag overrides reset");
                this.featureService.ClearAllOverrides();
                this.BuildFeatureFlagSection(column);
            })
            {
                text = "Reset Overrides",
            };
            resetButton.AddToClassList("settings-button");
            section.Add(resetButton);
        }

        private VisualElement BuildFeatureFlagRow(string feature)
        {
            var row = new VisualElement { name = "settings-row" };

            var leftColumn = new VisualElement { name = "left-column" };
            leftColumn.Add(new Label(feature));
            row.Add(leftColumn);

            var rightColumn = new VisualElement { name = "right-column" };
            var buttonRow = new VisualElement { name = "row" };

            Button toggleButton = null;
            toggleButton = new Button(() =>
            {
                var enabled = !this.featureService.IsEnabled(feature);
                this.featureService.SetOverride(feature, enabled);
                this.telemetryService.LogInfo($"Feature flag {feature} overridden to {enabled}");
                this.UpdateFeatureFlagButton(toggleButton, feature);
            });
            toggleButton.AddToClassList("settings-button");
            toggleButton.AddToClassList("selection-box");
            this.UpdateFeatureFlagButton(toggleButton, feature);

            buttonRow.Add(toggleButton);
            rightColumn.Add(buttonRow);
            row.Add(rightColumn);
            return row;
        }

        private void UpdateFeatureFlagButton(Button button, string feature)
        {
            // The asterisk marks a flag that has been forced either way, to distinguish it
            // from one still sitting at whatever this build type defaults to.
            var state = this.featureService.IsEnabled(feature) ? "On" : "Off";
            var marker = this.featureService.HasOverride(feature) ? " *" : string.Empty;
            button.text = state + marker;
        }
#endif
    }
}
