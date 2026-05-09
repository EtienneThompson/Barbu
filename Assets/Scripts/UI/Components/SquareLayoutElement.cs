using UnityEngine;
using UnityEngine.UI;

namespace Barbu.UI.Components
{
    [ExecuteAlways]
    [RequireComponent(typeof(LayoutElement))]
    public class SquareLayoutElement : MonoBehaviour
    {
        private LayoutElement layoutElement;
        private RectTransform rectTransform;
        private float lastWidth;

        void Awake()
        {
            this.layoutElement = GetComponent<LayoutElement>();
            this.rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            float width = this.rectTransform.rect.width;
            if (!Mathf.Approximately(this.lastWidth, width))
            {
                this.lastWidth = width;
                this.layoutElement.preferredHeight = width;
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            }
        }
    }
}