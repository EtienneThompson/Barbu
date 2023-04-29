using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuButtonsController : MonoBehaviour
{
    private Button settingsBtn;
    private Button gamesBtn;

    public void OnEnable()
    {
        GameObject menuButtons = GameObject.Find("MenuButtons");
        var document = menuButtons.GetComponent<UIDocument>();
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
    }

    private void HandleGamesButtonClick(ClickEvent evt)
    {
        Debug.Log("Handling games button click");
    }
}
