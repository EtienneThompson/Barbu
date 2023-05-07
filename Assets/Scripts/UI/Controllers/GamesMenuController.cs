using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamesMenuController : MonoBehaviour
{
    private StateMachine stateMachine;
    private GameBoard gameBoard;
    private GameObject gamesMenu;
    private GameObject menuButtons;
    private Button traditionalBtn;
    private Button singleBtn;
    private Button chaosBtn;
    private Button closeBtn;

    public void OnEnable()
    {
        this.stateMachine = new StateMachine();
        this.stateMachine.SetMenuOpen(true);
        this.gameBoard = GameObject.Find(Constants.GameObjects.GameBoard).GetComponent<GameBoard>();
        this.menuButtons = GameObject.Find(Constants.GameObjects.MenuButtons);
        this.gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
        var document = this.gamesMenu.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.traditionalBtn = root.Q<Button>("traditional");
        this.singleBtn = root.Q<Button>("single");
        this.chaosBtn = root.Q<Button>("chaos");
        this.closeBtn = root.Q<Button>("close");

        this.traditionalBtn.RegisterCallback<ClickEvent>(HandleTraditionalButtonClick);
        this.singleBtn.RegisterCallback<ClickEvent>(HandleSingleButtonClick);
        this.chaosBtn.RegisterCallback<ClickEvent>(HandleChaosButtonClick);
        this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
    }

    public void OnDisable()
    {
        this.stateMachine.SetMenuOpen(false);
        this.traditionalBtn.UnregisterCallback<ClickEvent>(HandleTraditionalButtonClick);
        this.singleBtn.UnregisterCallback<ClickEvent>(HandleSingleButtonClick);
        this.chaosBtn.UnregisterCallback<ClickEvent>(HandleChaosButtonClick);
        this.closeBtn.UnregisterCallback<ClickEvent>(HandleCloseButtonClick);
    }

    private void HandleTraditionalButtonClick(ClickEvent evt)
    {
        Debug.Log("Traditional button clicked");
        this.gameBoard.CreateNewGame(Constants.TraditionalRoundManager.GameName);
    }

    private void HandleSingleButtonClick(ClickEvent evt)
    {
        Debug.Log("Single button clicked");
        this.gameBoard.CreateNewGame(Constants.SingleRoundManager.GameName);
    }

    private void HandleChaosButtonClick(ClickEvent evt)
    {
        Debug.Log("Chaos button clicked");
        this.gameBoard.CreateNewGame(Constants.ChaosRoundManager.GameName);
    }

    private void HandleCloseButtonClick(ClickEvent evt)
    {
        Debug.Log("Close button clicked");
        this.gamesMenu.SetActive(false);
    }
}
