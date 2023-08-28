using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{
    private StateMachine stateMachine;
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

    public void OnEnable()
    {
        this.stateMachine = new StateMachine();
        this.stateMachine.SetMenuOpen(true);
        this.settingsMenu = GameObject.Find(Constants.GameObjects.SettingsMenu);
        var document = this.settingsMenu.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.sortingPreviousBtn = root.Q<Button>("sortingPrevious");
        this.sortingNextBtn = root.Q<Button>("sortingNext");
        this.difficultyPreviousBtn = root.Q<Button>("difficultyPrevious");
        this.difficultyNextBtn = root.Q<Button>("difficultyNext");
        this.closeBtn = root.Q<Button>("close");

        this.currentlySelectedSortingOption = root.Q<Label>("currentlySelectedSortingOption");
        this.currentlySelectedDifficulty = root.Q<Label>("currentlySelectedDifficulty");
        this.currentlySelectedSortingOption.text = Settings.SortingPreference.ToString();
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
        Debug.Log("Sorting Previous button clicked");
        this.currentSortingOption = this.SafeMod(this.currentSortingOption - 1, Settings.HandSortingOptions.Length);
        this.currentlySelectedSortingOption.text = Settings.HandSortingOptions[this.currentSortingOption].ToString();
        Settings.SortingPreference = Settings.HandSortingOptions[this.currentSortingOption];
    }

    private void HandleSortingNextButtonClick(ClickEvent evt)
    {
        Debug.Log("Sorting Next button clicked");
        this.currentSortingOption = this.SafeMod(this.currentSortingOption + 1, Settings.HandSortingOptions.Length);
        this.currentlySelectedSortingOption.text = Settings.HandSortingOptions[this.currentSortingOption].ToString();
        Settings.SortingPreference = Settings.HandSortingOptions[this.currentSortingOption];
    }

    private void HandleDifficultyPreviousButtonClick(ClickEvent evt)
    {
        Debug.Log("Difficulty Previous button clicked");
        this.currentDifficultyOption = this.SafeMod(this.currentDifficultyOption - 1, Settings.ComputerDifficulties.Length);
        this.currentlySelectedDifficulty.text = Settings.ComputerDifficulties[this.currentDifficultyOption].ToString();
        Settings.ComputerDifficultyPreference = Settings.ComputerDifficulties[this.currentDifficultyOption];
    }

    private void HandleDifficultyNextButtonClick(ClickEvent evt)
    {
        Debug.Log("Difficulty Next Button clicked");
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
