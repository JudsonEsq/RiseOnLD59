using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public List<Collider> pheromonesToTrack; // List to store detected pheromones
    public List<Collider> pheromonesToAvoid; // List to store detected pheromones
    public List<Collider> previousPheromones = new(); // List to store previously detected pheromones to avoid re-targeting
    public Collider chosenPheromone; // Variable to store the chosen pheromone detected
    public float pheromoneRange = 20f; // Range within which the ant can detect pheromones
    public bool isMovingTowardsPheromone = false; // Flag to indicate if the ant is currently moving towards a pheromone
    private Vector3 targetPosition; // Variable to store the target position for movement
    public float searchTimer = 1f; // Time in seconds between pheromones searches
    public float searchDuration = 5f; // Time in seconds to search for new pheromones before returning to nest
    private float timeSinceLastSearch = 0f; // Time in seconds since the last pheromone search
    private float timeSinceLastFind = 0f; // Time in seconds since the last pheromone was found
    public GameObject nest; // Reference to the nest GameObject for returning after searching for pheromones
    public float moveSpeed = 2f; // Speed at which the ant moves towards pheromones and the nest
    public bool isReturningToNest = false; // Flag to indicate if the ant is currently returning to the nest
    public List<Pheromone.PheromoneType> targetPheromones = new();
    public List<Pheromone.PheromoneType> avoidPheromones = new();
    public int maxPheromonesCount = 3; // Maximum number of pheromones to consider when picking a target

    public bool isAlive = true;
    public int heldFood = 0;
	
	private Animator anim;

    [Serializable]
    public enum AntType
    {
        Worker,
        Soldier,
        Fire,
        Carpenter,
    }

    [SerializeField]
    public AntType antType;

    [SerializeField]
    public int foodCost = 1;

    public void Init()
    {
        FindPheromones();
        isMovingTowardsPheromone = false;
		
		// anim = GetComponentInChildren<Animator>();
    }

    public void Operate()
    {
        if (IsDead()) return;

        if (isReturningToNest)
        {
			//anim.SetBool("isMoving", true);
            ReturnToNest();
            return;
        }

        if (isMovingTowardsPheromone)
        {
			// anim.SetBool("isMoving", true);
            MoveTowardsPheromone();
            return;
        }

        // Incremement search timers
        timeSinceLastSearch += Time.deltaTime;
        timeSinceLastFind += Time.deltaTime;

        // Search for pheromones at regular interval searchTimer
        if (timeSinceLastSearch >= searchTimer)
        {
            // Reset search timer
            timeSinceLastSearch = 0f;
            FindPheromones();

            // Search for pheromones and reset timeSinceLastFind if any are found
            if (pheromonesToTrack.Count() > 0)
            {
                timeSinceLastFind = 0f;
                // Debug.Log("Pheromones found, resetting search timer");

                PickPheromone();
            }

            else
            {
                Debug.Log("No pheromones found, continuing search");
                return;
            }
        }

        // If searching for pheromones for too long without finding any, return to nest
        if (timeSinceLastFind >= searchDuration)
        {
            ReturnToNest();
            return;
        }

        // If not currently moving towards a pheromone, pick one to move towards
        if (!isMovingTowardsPheromone)
        {
            if (chosenPheromone != null)
            {
                targetPosition = new Vector3 (chosenPheromone.transform.position.x, transform.position.y, chosenPheromone.transform.position.z);
                isMovingTowardsPheromone = true;
                Debug.Log("Chosen pheromone at position: " + targetPosition);
                MoveTowardsPheromone();
                return;
            }
        }
    }

    // Method to find pheromones within range and store them in the pheromones list
    public void FindPheromones()
    {
        List <Collider> pheromones = Physics.OverlapSphere(transform.position, pheromoneRange, LayerMask.GetMask("Pheromones")).ToList();
        pheromonesToTrack.Clear();
        pheromonesToAvoid.Clear();

        for (int i = 0; i < pheromones.Count(); i++)
        {
            // Filter out depleted food sources that are missing Pheromone component
            if (!pheromones[i].TryGetComponent(out Pheromone pheromone))
            {
                pheromones.RemoveAt(i);
                i--;
                Debug.Log("Removed old food source");
                continue;
            }

            // Filter out pheromones that have previously been visited along this path
            if (previousPheromones.Contains(pheromones[i]))
            {
                pheromones.RemoveAt(i);
                i--;
                continue;
            }

            // Add applicable pheromones to targetPheromones based on PheromoneType
            if (targetPheromones.Contains(pheromones[i].GetComponent<Pheromone>().pheromoneType))
            {
                pheromonesToTrack.Add(pheromones[i]);
            }

            // Add applicable pheromones to avoidPheromones based on PheromoneType
            else if (avoidPheromones.Contains(pheromones[i].GetComponent<Pheromone>().pheromoneType))
            {
                pheromonesToAvoid.Add(pheromones[i]);
            }
        }
    }

    // Method to pick the chosen pheromone based on proximity and type
    public void PickPheromone()
    {
        chosenPheromone = null;

        // Check is there are pheromonesToTrack
        if (pheromonesToTrack == null || pheromonesToTrack.Count() == 0)
        {
            return; // No pheromones found
        }

        if (pheromonesToTrack.Count() == 1)
        {
            chosenPheromone = pheromonesToTrack[0];
            return;
        }

        Collider[] pheromonesArray = pheromonesToTrack.ToArray();
        Array.Sort(pheromonesArray, (a, b) => a.GetComponent<Pheromone>().GetDistanceToAnt(this).CompareTo(b.GetComponent<Pheromone>().GetDistanceToAnt(this)));
        int pheromonesCount = pheromonesArray.Length > maxPheromonesCount ? maxPheromonesCount : pheromonesArray.Length;
        Debug.Log("Pheromones Count: " + pheromonesCount);

        float distanceSum = 0f;

        for (int i = 0; i < pheromonesCount; i++)
        {
            distanceSum += pheromonesArray[i].GetComponent<Pheromone>().GetDistanceToAnt(this);
        }

        float[] distanceThresholds = new float[pheromonesCount];
        float thresholdSum = 0f;
        float safetyFactor;
        float [] distanceToHazards = new float[pheromonesToAvoid.Count()];

        for (int i = 0; i < pheromonesCount; i++)
        {
            // Temporarily store the distances between the current pheromoneToTrack and all pheromonesToAvoid
            for (int j = 0; j < pheromonesToAvoid.Count(); j++)
            {
                distanceToHazards[j] = (pheromonesToAvoid[j].transform.position - pheromonesArray[i].transform.position).sqrMagnitude;
            }

            safetyFactor = CalculateSafetyFactor(Mathf.Min(distanceToHazards));
            distanceThresholds[i] = (distanceSum - pheromonesArray[distanceThresholds.Count() - 1 - i].GetComponent<Pheromone>().GetDistanceToAnt(this)) / distanceSum * safetyFactor;
            thresholdSum += distanceThresholds[i];
        }

        foreach (float threshold in distanceThresholds)
        {
            Debug.Log("Distance Threshold: " + threshold);
        }

        float[] priorityThresholds = new float[distanceThresholds.Length];

        for (int i = 0; i < priorityThresholds.Length; i++)
        {
            priorityThresholds[i] = distanceThresholds[i] / thresholdSum;

            if (i > 0)
            {
                priorityThresholds[i] += priorityThresholds[i - 1];
            }
        }

        foreach (float threshold in priorityThresholds)
        {
            Debug.Log("Priority Threshold: " + threshold);
        }

        float tempValue = UnityEngine.Random.value;
        Debug.Log("Random Value: " + tempValue);

        // NEED TO ADD RECURSIVE LOGIC TO ADJUST BASED ON COUNT

        if (tempValue <= priorityThresholds[0])
        {
            chosenPheromone = pheromonesToTrack[0];
        }

        else if (tempValue <= priorityThresholds[1])
        {
            chosenPheromone = pheromonesToTrack[1];
        }

        else
        {
            chosenPheromone = pheromonesToTrack[2];
        }
    }

    private float CalculateSafetyFactor(float distance)
    {
        if (distance >= 100f)
        {
            return 1f;
        }
        else if (distance >= 25f)
        {
            return 0.75f;
        }
        else if (distance >= 9f)
        {
            return 0.5f;
        }
        else
        {
            return 0.25f;
        }
    }

    public void MoveTowardsPheromone()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        // Jude code hoping to rotate ant mesh towards direction of movement
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += moveSpeed * Time.deltaTime * direction;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
			//anim.SetBool("isMoving", false);
            // Reached the pheromone
            Debug.Log("Reached the pheromone");
            CheckForFood(chosenPheromone);
            previousPheromones.Add(chosenPheromone);
            chosenPheromone = null;
            isMovingTowardsPheromone = false;
            timeSinceLastSearch = 0f;

            if (IsCarryingFood())
            {
                ReturnToNest();
            }
            else
            {
                FindPheromones();
            }

            return;
        }

        MoveTowardsTarget();
    }

    public void CheckForFood(Collider pheromone)
    {
        if (pheromone.TryGetComponent(out FoodSource foodSource))
        {
            if (foodSource.currentFood > 0)
            {
                int foodToPickup = Mathf.Min(foodSource.foodValue, foodSource.currentFood);

                if (foodSource.isDepletable)
                {
                    foodSource.currentFood -= foodToPickup;
                }

                PickupFood(foodToPickup);
                Debug.Log("Picked up " + foodToPickup + " food from the source");
            }
            else
            {
                Debug.Log("No food left at the source");
            }
        }
    }

    public void ReturnToNest()
    {
        isReturningToNest = true;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (targetPosition == nest.transform.position)
            {
                Debug.Log("Returned to the nest");
                timeSinceLastFind = 0f;
                timeSinceLastSearch = 0f;
                previousPheromones.Clear();
                isReturningToNest = false;
                // Food += heldFood;
                heldFood = 0;
                return;
            }

            if (previousPheromones.Count() == 0 || previousPheromones == null)
            {
                targetPosition = nest.transform.position;
            }

            else
            {
                int pheromoneIndex = previousPheromones.Count() - 1;
                Collider targetCollider = previousPheromones[pheromoneIndex];
                targetPosition = new Vector3 (targetCollider.transform.position.x, transform.position.y, targetCollider.transform.position.z);
                previousPheromones.RemoveAt(pheromoneIndex);
            }
        }

        MoveTowardsTarget();
    }

    public void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    public bool IsDead()
    {
        return !this.isAlive;
    }

    public void Kill()
    {
        Debug.Log("Killing Ant");
        this.isAlive = false;
    }

    public int FoodCost()
    {
        return foodCost;
    }

    public bool IsCarryingFood()
    {
        return heldFood > 0;
    }

    public void PickupFood(int foodAmount)
    {
        heldFood = foodAmount;
    }
}
