using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{
    private StateMachine stateMachine;
    private GameObject settingsMenu;
    private GameObject menuButtons;
    private Button sortingPreviousBtn;
    private Button sortingNextBtn;
    private Button closeBtn;
    private Label currentlySelectedSortingOption;
    private int currentSortingOption;

    public void OnEnable()
    {
        this.stateMachine = new StateMachine();
        this.stateMachine.SetMenuOpen(true);
        this.menuButtons = GameObject.Find(Constants.GameObjects.MenuButtons);
        this.settingsMenu = GameObject.Find(Constants.GameObjects.SettingsMenu);
        var document = this.settingsMenu.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.sortingPreviousBtn = root.Q<Button>("sortingPrevious");
        this.sortingNextBtn = root.Q<Button>("sortingNext");
        this.closeBtn = root.Q<Button>("close");

        this.currentlySelectedSortingOption = root.Q<Label>("currentlySelectedSortingOption");
        this.currentlySelectedSortingOption.text = Settings.SortingPreference.ToString();
        this.currentSortingOption = Array.IndexOf(Settings.HandSortingOptions, Settings.SortingPreference);

        this.sortingPreviousBtn.RegisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
        this.sortingNextBtn.RegisterCallback<ClickEvent>(HandleSortingNextButtonClick);
        this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
    }

    public void OnDisable()
    {
        this.stateMachine.SetMenuOpen(false);
        this.sortingPreviousBtn.UnregisterCallback<ClickEvent>(HandleSortingPreviousButtonClick);
        this.sortingNextBtn.UnregisterCallback<ClickEvent>(HandleSortingNextButtonClick);
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
