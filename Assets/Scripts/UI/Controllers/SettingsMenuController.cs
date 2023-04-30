using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{
    private GameObject settingsMenu;
    private GameObject menuButtons;
    private Button closeBtn;

    public void OnEnable()
    {
        Debug.Log("SettingsMenuController OnEnable");
        this.menuButtons = GameObject.Find("MenuButtons");
        this.settingsMenu = GameObject.Find("SettingsMenu");
        var document = this.settingsMenu.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.closeBtn = root.Q<Button>("close");

        this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
    }

    public void OnDisable()
    {
        Debug.Log("SettingsMenuController OnDisable");
        this.closeBtn.UnregisterCallback<ClickEvent>(HandleCloseButtonClick);
    }

    private void HandleCloseButtonClick(ClickEvent evt)
    {
        Debug.Log("Close button clicked");
        this.settingsMenu.SetActive(false);
    }
}
