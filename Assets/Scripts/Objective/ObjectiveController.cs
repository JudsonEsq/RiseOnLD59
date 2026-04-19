using Assets.Scripts.Objective;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    private const string OBJECTIVE_DISPLAY_HEADER = "<size=64>Objective(s):</size>";

    TextMeshProUGUI ObjectiveDisplay;

    ObjectiveContainer ObjectiveContainer;

    DateTime completionTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Create Objectives Here
        ObjectiveContainer = new ObjectiveContainer();

        ObjectiveDisplay = GetComponentInChildren<TextMeshProUGUI>();

        foreach(int i in Enumerable.Range(1,2))
        {
            ObjectiveContainer.TryAddObjective(new Objective { Step = i, Name = $"Objective {i}", Description = $"Objective #{i}." });

            Debug.Log($"Added Objective #{i}.");
        }

        ObjectiveContainer.TryAddObjective(new Objective { Step = 3, Name = "Objective 3", Description = "Objective #3" });
        ObjectiveContainer.TryAddObjective(new Objective { Step = 4, Name = "Objective 4", Description = "Objective #4" });

        ShowObjective(true);

        completionTime = DateTime.Now + TimeSpan.FromSeconds(10);
    }

    // Update is called once per frame
    void Update()
    {
        ObjectiveDisplay.text = OBJECTIVE_DISPLAY_HEADER;

        foreach (Objective objective in ObjectiveContainer.ObjectiveList)
        {
            string line = $"\n- {objective.Name}:\n\t";

            if (objective.Complete)
            {
                line += $"- <s>{objective.Description}</s>";
            }
            else
            {
                line += $"- {objective.Description}";
            }

            ObjectiveDisplay.text = $"{ObjectiveDisplay.text}{line}";
        }

        if(DateTime.Now >= completionTime && !ObjectiveContainer.ObjectiveList[0].Complete)
        {
            ObjectiveContainer.CompleteObjective(ObjectiveContainer.ObjectiveList[0].Step);
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
    }

    /// <summary>
    /// Toggles the visibility of the objective UI element. This can be called when the player enters or exits an area, or when an objective is completed.
    /// </summary>
    /// <param name="show"></param>
    void ShowObjective(bool show)
    {
        ObjectiveDisplay.gameObject.SetActive(show);
    }
}
