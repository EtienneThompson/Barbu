using UnityEngine;
using UnityEngine.UIElements;

public class MenuButtonsController : MonoBehaviour
{
    private GameObject menuButtons;
    private GameObject gamesMenu;
    private GameObject settingsMenu;
    private GameObject howToPlayScreen;
    private Button settingsBtn;
    private Button gamesBtn;
    private Button howToPlayBtn;

    public void OnEnable()
    {
        this.gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
        this.settingsMenu = GameObject.Find(Constants.GameObjects.SettingsMenu);
        this.menuButtons = GameObject.Find(Constants.GameObjects.InGamePoints);
        this.howToPlayScreen = GameObject.Find(Constants.GameObjects.HowToPlayScreen);
        var document = this.menuButtons.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.settingsBtn = root.Q<Button>("settings");
        this.gamesBtn = root.Q<Button>("games");
        this.howToPlayBtn = root.Q<Button>("howtoplay");

        this.settingsBtn.RegisterCallback<ClickEvent>((e) => HandleSettingsButtonClick(e));
        this.gamesBtn.RegisterCallback<ClickEvent>(HandleGamesButtonClick);
        this.howToPlayBtn.RegisterCallback<ClickEvent>(HandleHowToPlayButtonClick);
    }

    public void OnDisable()
    {
        this.settingsBtn.UnregisterCallback<ClickEvent>(HandleSettingsButtonClick);
        this.gamesBtn.UnregisterCallback<ClickEvent>(HandleGamesButtonClick);
        this.howToPlayBtn.UnregisterCallback<ClickEvent>(HandleHowToPlayButtonClick);
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

    private void HandleHowToPlayButtonClick(ClickEvent evt)
    {
        Debug.Log("Handling how to play button click");
        this.howToPlayScreen.SetActive(true);
    }
}
