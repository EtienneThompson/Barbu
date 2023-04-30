using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuButtonsController : MonoBehaviour
{
    private GameObject menuButtons;
    private GameObject gamesMenu;
    private GameObject settingsMenu;
    private Button settingsBtn;
    private Button gamesBtn;

    public void OnEnable()
    {
        Debug.Log("MenuButtonsController OnEnable");
        this.gamesMenu = GameObject.Find("GamesMenu");
        this.settingsMenu = GameObject.Find("SettingsMenu");
        this.menuButtons = GameObject.Find("MenuButtons");
        Debug.Log(this.gamesMenu);
        Debug.Log(this.settingsMenu);
        var document = this.menuButtons.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.settingsBtn = root.Q<Button>("settings");
        this.gamesBtn = root.Q<Button>("games");

        this.settingsBtn.RegisterCallback<ClickEvent>((e) => HandleSettingsButtonClick(e));
        this.gamesBtn.RegisterCallback<ClickEvent>(HandleGamesButtonClick);
    }

    public void OnDisable()
    {
        this.settingsBtn.UnregisterCallback<ClickEvent>(HandleSettingsButtonClick);
        this.gamesBtn.UnregisterCallback<ClickEvent>(HandleGamesButtonClick);
    }

    private void HandleSettingsButtonClick(ClickEvent evt) 
    {
        Debug.Log("Settings button clicked");
        this.settingsMenu.SetActive(true);
    }

    private void HandleGamesButtonClick(ClickEvent evt)
    {
        Debug.Log("Handling games button click");
        this.gamesMenu.SetActive(true);
    }
}
