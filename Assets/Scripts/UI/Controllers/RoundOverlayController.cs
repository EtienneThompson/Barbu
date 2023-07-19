using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundOverlayController : MonoBehaviour
{
    private EventsController eventsController;
    private Label roundLabel;

    public void OnEnable()
    {
        this.eventsController = EventsController.GetInstance();
        var roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
        var document = roundOverlay.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.roundLabel = root.Q<Label>("round");
    }

    public void DisplayRound(string roundName)
    {
        StartCoroutine(DisplayRoutine(roundName));
    }

    IEnumerator DisplayRoutine(string roundName)
    {
        Debug.Log(roundName);
        this.roundLabel.text = roundName;
        yield return new WaitForSeconds(1.5f);
        this.roundLabel.text = string.Empty;
        this.eventsController.FinishRoundAnimation();
    }
}
