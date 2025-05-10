namespace Barbu.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using Barbu.Core;
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
        private GameObject settingsMenu;
        private Button sortingPreviousBtn;
        private Button sortingNextBtn;
        private Button difficultyPreviousBtn;
        private Button difficultyNextBtn;
        private Button closeBtn;
        private Label currentlySelectedSortingOption;
        private Label currentlySelectedDifficulty;
        private int currentSortingOption;
        private int currentDifficultyOption;

        [Inject]
        public void Init(IStateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            this.stateMachine.SetMenuOpen(true);
            this.settingsMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SettingsMenu, findInactive: true);
            var document = this.settingsMenu.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.sortingPreviousBtn = root.Q<Button>("sortingPrevious");
            this.sortingNextBtn = root.Q<Button>("sortingNext");
            this.difficultyPreviousBtn = root.Q<Button>("difficultyPrevious");
            this.difficultyNextBtn = root.Q<Button>("difficultyNext");
            this.closeBtn = root.Q<Button>("close");

            this.currentlySelectedSortingOption = root.Q<Label>("currentlySelectedSortingOption");
            this.currentlySelectedDifficulty = root.Q<Label>("currentlySelectedDifficulty");
            _ = HandSortingStrings.TryGetValue(Settings.SortingPreference.ToString(), out var initialSortingOptionString);
            this.currentlySelectedSortingOption.text = initialSortingOptionString;
            this.currentlySelectedSortingOption.style.unityFontDefinition = FontDefinition.FromFont(Resources.Load<Font>("Fonts/LucidaSansUnicodeRegular"));
            this.currentSortingOption = Array.IndexOf(Settings.HandSortingOptions, Settings.SortingPreference);
            this.currentlySelectedDifficulty.text = Settings.ComputerDifficultyPreference.ToString();
            this.currentDifficultyOption = Array.IndexOf(Settings.ComputerDifficulties, Settings.ComputerDifficultyPreference);

            this.sortingPreviousBtn.RegisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
            this.sortingNextBtn.RegisterCallback<ClickEvent>(HandleSortingNextButtonClick);
            this.difficultyPreviousBtn.RegisterCallback<ClickEvent>(HandleDifficultyPreviousButtonClick);
            this.difficultyNextBtn.RegisterCallback<ClickEvent>(HandleDifficultyNextButtonClick);
            this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        public void OnDisable()
        {
            this.stateMachine.SetMenuOpen(false);
            this.sortingPreviousBtn.UnregisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
            this.sortingNextBtn.UnregisterCallback<ClickEvent>(HandleSortingNextButtonClick);
            this.difficultyPreviousBtn.UnregisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
            this.difficultyNextBtn.UnregisterCallback<ClickEvent>(HandleDifficultyNextButtonClick);
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

        private void HandleCloseButtonClick(ClickEvent evt)
        {
            this.settingsMenu.SetActive(false);
        }

        private int SafeMod(int numerator, int modulus)
        {
            int result = numerator % modulus;
            return result < 0 ? result + modulus : result;
        }
    }
}
