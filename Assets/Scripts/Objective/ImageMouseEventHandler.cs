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

    DateTime MouseExitedObjectivePanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Entered Objective Background.");

        MouseInObjectivePanel = true;

        ObjectiveDisplay.color = new Color(ObjectiveDisplay.color.r, ObjectiveDisplay.color.g, ObjectiveDisplay.color.b, 1.0f);
        ObjectiveBackground.color = new Color(ObjectiveBackground.color.r, ObjectiveBackground.color.g, ObjectiveBackground.color.b, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exited Objective Background.");

        MouseInObjectivePanel = false;
        MouseExitedObjectivePanel = DateTime.Now;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectiveDisplay = GetComponentInChildren<TextMeshProUGUI>();
        ObjectiveBackground = GetComponentInChildren<Image>();

        Debug.Log("ObjectiveBackground: " + ObjectiveBackground == null ? "Null" : "Not Null");
    }

    // Update is called once per frame
    void Update()
    {
        if(!MouseInObjectivePanel)
        {
            Color ObjectiveColor = ObjectiveDisplay.color;
            Color BackgroundColor = ObjectiveBackground.color;

            float Opacity = Math.Max(.2f, 1 - (float)(DateTime.Now - MouseExitedObjectivePanel).TotalSeconds / 10);

            ObjectiveDisplay.color = new Color(ObjectiveColor.r, ObjectiveColor.g, ObjectiveColor.b, Opacity);
            ObjectiveBackground.color = new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, Opacity);
        }
    }
}
