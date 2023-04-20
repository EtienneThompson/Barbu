using UnityEngine;
using System.Collections;

public class HighlightObject : MonoBehaviour{
    private Color initialColor;
    public bool noEmissionAtStart = true;
    public Color highlightColor = Color.yellow;
    public Color mousedownColor = Color.red;

    private bool mouseon = false;
    private Renderer myRenderer;

    void Start() {
        myRenderer = GetComponent<Renderer>();
        if (noEmissionAtStart)
        initialColor = Color.black;
        else
        initialColor = myRenderer.material.GetColor("_EmissionColor");
    }

    void OnMouseEnter(){
        mouseon = true;
        myRenderer.material.SetColor("_EmissionColor", highlightColor);
    }

    void OnMouseExit(){
        mouseon = false;
        myRenderer.material.SetColor("_EmissionColor",initialColor);
    }

    void OnMouseDown(){
        myRenderer.material.SetColor("_EmissionColor", mousedownColor);
    }

    void OnMouseUp(){
        if (mouseon)
        myRenderer.material.SetColor("_EmissionColor", highlightColor);
        else
        myRenderer.material.SetColor("_EmissionColor", initialColor);
    }
}