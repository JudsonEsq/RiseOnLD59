using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objective
{
    internal class ObjectiveContainer
    {
        private List<Objective> Objectives = new List<Objective>();

        public IReadOnlyList<Objective> ObjectiveList => Objectives.OrderBy(o => o.Step).ToList();

        public int Count => Objectives.Count;

        public ObjectiveContainer()
        {
            Debug.Log($"[{nameof(ObjectiveContainer)}] Initialized ObjectiveContainer.");
        }

        /// <summary>
        /// Removes all objectives from the collection.
        /// </summary>
        /// <remarks>After calling this method, the collection of objectives will be empty. This operation
        /// does not raise an exception if the collection is already empty.</remarks>
        public void ClearObjectives()
        {
            Objectives.Clear();

            Debug.Log($"[{nameof(ObjectiveContainer)}] Cleared all objectives.");
        }

        /// <summary>
        /// Marks the objective associated with the specified step as complete, if it exists.
        /// </summary>
        /// <remarks>If no objective with the specified step exists, this method performs no
        /// action.</remarks>
        /// <param name="step">The step number of the objective to mark as complete.</param>
        public void CompleteObjective(int step)
        {
            Objective obj = Objectives.FirstOrDefault(o => o.Step == step);
            if(obj != null)
            {
                var timeCompleted = DateTime.Now;

                obj.Complete = true;
                obj.TimeCompleted = timeCompleted;

                Debug.Log($"[{nameof(ObjectiveContainer)}] Marked Complete at {timeCompleted}: {obj.Name} (Step {obj.Step}) - {obj.Description}");
            }
        }

        /// <summary>
        /// Marks the specified objective as complete.
        /// </summary>
        /// <param name="obj">The objective to be marked as complete. Cannot be null.</param>
        public void CompleteObjective(Objective obj)
        {
            if(ContainsObjective(obj))
            {
                CompleteObjective(obj.Step);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Objective " + obj.Name + " not in current container.");
            }
        }

        /// <summary>
        /// Determines whether the collection contains an objective with the same step as the specified objective.
        /// </summary>
        /// <param name="obj">The objective to compare against the objectives in the collection. The method checks for an objective with a
        /// matching step.</param>
        /// <returns>true if an objective with the same step as the specified objective exists in the collection; otherwise,
        /// false.</returns>
        public bool ContainsObjective(Objective obj) => Objectives.Any(o => o.Step == obj.Step);

        /// <summary>
        /// Attempts to add the specified objective to the container if no objective with the same step already exists.
        /// </summary>
        /// <remarks>Only one objective per step is allowed in the container. If an objective with the
        /// same step already exists, this method does not add the new objective and returns false.</remarks>
        /// <param name="objective">The objective to add to the container. Must not be null and must have a unique step value.</param>
        /// <returns>true if the objective was added successfully; otherwise, false.</returns>
        public bool TryAddObjective(Objective objective)
        {
            // If an Objective with the given Step already exists, return false.
            // We can only have a single objective per step.
            if(ContainsObjective(objective))
            {
                return false;
            }

            Debug.Log($"[{nameof(ObjectiveContainer)}] Adding objective: {objective.Name} (Step {objective.Step}) - {objective.Description}");

            Objectives.Add(objective);

            return true;
        }

        /// <summary>
        /// Attempts to remove the objective associated with the specified step number.
        /// </summary>
        /// <param name="step">The step number of the objective to remove.</param>
        /// <returns>true if an objective with the specified step was found and removed; otherwise, false.</returns>
        public bool TryRemoveObjective(int step)
        {
            var objectiveToRemove = Objectives.FirstOrDefault(o => o.Step == step);

            if (objectiveToRemove != null)
            {
                Objectives.Remove(objectiveToRemove);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove the specified objective from the collection.
        /// </summary>
        /// <param name="objective">The objective to remove from the collection. Cannot be null.</param>
        /// <returns>true if the objective was successfully removed; otherwise, false.</returns>
        public bool TryRemoveObjective(Objective objective) => TryRemoveObjective(objective.Step);
    }
}
