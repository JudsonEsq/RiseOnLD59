using Assets.Scripts.Objective;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour
{
    public AudioHUD audioHUD;
    private const string OBJECTIVE_DISPLAY_HEADER = "Objective(s):";

    [SerializeField]
    int HeaderTextSize = 64;

    [SerializeField]
    int ObjectiveTextSize = 36;

    [SerializeField]
    string ObjectiveCompletedColor = "green";

    [SerializeField]
    int SecsBeforeRemoveCompletedObjective = 5;

    TextMeshProUGUI ObjectiveDisplay;

    ObjectiveContainer ObjectiveContainer;

    ImageMouseEventHandler ImageMouseEventHandler;

    void Awake()
    {
        // Create Objectives Here
        ObjectiveContainer = new ObjectiveContainer();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectiveDisplay = GetComponentInChildren<TextMeshProUGUI>();
        ImageMouseEventHandler = GetComponentInParent<ImageMouseEventHandler>();
        ImageMouseEventHandler.SetOpacity(1.0f);

        ShowObjective(true);
    }

    // Update is called once per frame
    void Update()
    {
        ObjectiveDisplay.text = $"<size={HeaderTextSize}>{OBJECTIVE_DISPLAY_HEADER}</size>";

        foreach (Objective objective in ObjectiveContainer.ObjectiveList)
        {
            string line = $"\n- {objective.Name}:\n\t";

            if (objective.Complete)
            {
                if(DateTime.Now > objective.TimeCompleted + TimeSpan.FromSeconds(SecsBeforeRemoveCompletedObjective))
                {
                    // Remove the completed objective from the objective container if it's been long enough since it was completed. This will prevent the display from being cluttered with completed objectives.
                    ObjectiveContainer.TryRemoveObjective(objective);

                    continue;
                }

                line += $"<color=\"{ObjectiveCompletedColor}\">- {objective.Description}</color>";
            }
            else
            {
                line += $"- {objective.Description}";
            }

            line = $"<size={ObjectiveTextSize}>{line}</size>";

            ObjectiveDisplay.text += $"{line}";
        }
    }

    /// <summary>
    /// Removes all objectives from the objective container.
    /// </summary>
    /// <remarks>Call this method to reset the objective list to an empty state. This operation cannot be
    /// undone.</remarks>
    void ClearObjectives()
    {
        ObjectiveContainer.ClearObjectives();
    }

    /// <summary>
    /// Set the current objective to the provided objective. This method will be responsible for updating the UI elements to reflect the new objective's name and description.
    /// </summary>
    /// <param name="objective">The objective to activate.</param>
    void SetObjective(Objective objective)
    {
        // Set the current objective to the provided objective
        // This involves updating the UI elements to reflect the new objective's name and description.
        ObjectiveContainer.TryAddObjective(objective);
    }

    /// <summary>
    /// Toggles the visibility of the objective UI element. This can be called when the player enters or exits an area, or when an objective is completed.
    /// </summary>
    /// <param name="show"></param>
    void ShowObjective(bool show)
    {
        ObjectiveDisplay.gameObject.SetActive(show);
    }

    public void AddObjective(int step, string name, string Description)
    {
        ObjectiveContainer.TryAddObjective(new Objective { Step = step, Name = name, Description = Description });
    }

    public void CompleteObjective(int step)
    {
        ObjectiveContainer.CompleteObjective(step, audioHUD);
    }
}
