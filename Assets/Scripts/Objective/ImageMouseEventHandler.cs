using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageMouseEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI ObjectiveDisplay;
    Image ObjectiveBackground;

    bool MouseInObjectivePanel = false;

    DateTime? MouseExitedObjectivePanel;

    [SerializeField]
    int FadeOutTimeSeconds = 10;

    [SerializeField]
    int LoadDelayBeforeInitialFadeSeconds = 20;

    [SerializeField]
    float MinimumOpacity = .2f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Entered Objective Background.");

        MouseInObjectivePanel = true;

        SetOpacity(1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exited Objective Background.");

        MouseInObjectivePanel = false;
        MouseExitedObjectivePanel = DateTime.Now;
    }

    void Awake()
    {
        ObjectiveDisplay = GetComponentInChildren<TextMeshProUGUI>();
        ObjectiveBackground = GetComponentInChildren<Image>();

        Debug.Log("ObjectiveBackground: " + (ObjectiveBackground == null ? "Null" : "Not Null"));

        SetOpacity(1.0f);

        MouseExitedObjectivePanel = DateTime.Now + TimeSpan.FromSeconds(LoadDelayBeforeInitialFadeSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        if(!MouseInObjectivePanel && MouseExitedObjectivePanel.HasValue)
        {
            float Opacity = Math.Max(MinimumOpacity, 1 - (float)(DateTime.Now - MouseExitedObjectivePanel.Value).TotalSeconds / FadeOutTimeSeconds);

            SetOpacity(Opacity);
        }
    }

    internal void SetOpacity(float opacity)
    {
        Color ObjectiveColor = ObjectiveDisplay.color;
        Color BackgroundColor = ObjectiveBackground.color;

        ObjectiveDisplay.color = new Color(ObjectiveColor.r, ObjectiveColor.g, ObjectiveColor.b, opacity);
        ObjectiveBackground.color = new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, opacity);
    }
}
