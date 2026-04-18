using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public List<Collider> pheromones; // List to store detected pheromones
    private List<Collider> previousPheromones = new List<Collider>(); // List to store previously detected pheromones to avoid re-targeting
    private Collider chosenPheromone; // Variable to store the chosen pheromone detected
    public float pheromoneRange = 20f; // Range within which the ant can detect pheromones
    public bool isMovingTowardsPheromone; // Flag to indicate if the ant is currently moving towards a pheromone
    private Vector3 targetPosition; // Variable to store the target position for movement
    public float searchTimer = 1f; // Time in seconds between pheromones searches
    public float searchDuration = 5f; // Time in seconds to search for new pheromones before returning to nest
    private float timeSinceLastSearch = 0f; // Time in seconds since the last pheromone search
    private float timeSinceLastFind = 0f; // Time in seconds since the last pheromone was found
    public GameObject nest; // Reference to the nest GameObject for returning after searching for pheromones
    public float moveSpeed = 2f; // Speed at which the ant moves towards pheromones and the nest
    public bool isReturningToNest = false; // Flag to indicate if the ant is currently returning to the nest

    public bool isAlive = true;
    public int heldFood = 0;

    [SerializeField]
    public int foodCost = 1;

    void Start()
    {
        FindPheromones();
        isMovingTowardsPheromone = false;
    }

    void Update()
    {
        if (isReturningToNest)
        {
            ReturnToNest();
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

            // Search for pheromones and reset timeSinceLastFind if any are found
            if (FindPheromones())
            {
                timeSinceLastFind = 0f;
                Debug.Log("Pheromones found, resetting search timer");
            }
            else
            {
                Debug.Log("No pheromones found, continuing search");
            }
        }

        // If searching for pheromones for too long without finding any, return to nest
        if (timeSinceLastFind >= searchDuration)
        {
            ReturnToNest();
            return;
        }

        // If no pheromones are found, stay in position
        if (pheromones == null || pheromones.Count() == 0)
        {
            return;
        }

        // If not currently moving towards a pheromone, pick one to move towards
        if (!isMovingTowardsPheromone)
        {
            int chosenPheromoneNum = PickPheromone();
            if (chosenPheromoneNum != -1)
            {
                chosenPheromone = pheromones[chosenPheromoneNum];
                targetPosition = new Vector3 (chosenPheromone.transform.position.x, transform.position.y, chosenPheromone.transform.position.z);
                isMovingTowardsPheromone = true;
                Debug.Log("Chosen pheromone at position: " + targetPosition);
            }
        }
        
        if (isMovingTowardsPheromone)
        {
            MoveTowardsPheromone();
        }
        
    }

    // Method to find pheromones within range and store them in the pheromones list
    public bool FindPheromones()
    {
        pheromones = Physics.OverlapSphere(transform.position, pheromoneRange, LayerMask.GetMask("Pheromones")).ToList();
        for (int i = 0; i < pheromones.Count(); i++)
        {
            if (!pheromones[i].TryGetComponent(out Pheromone pheromone))
            {
                Debug.Log("Removed non-pheromone collider: " + pheromones[i].name);
                pheromones.RemoveAt(i);
                i--;
                continue;
            }

            if (previousPheromones.Contains(pheromones[i]))
            {
                pheromones.RemoveAt(i);
                i--;
            }
        }

        return pheromones.Count() > 0 ? true : false;
    }

    // Method to pick the chosen pheromone based on proximity and type
    public int PickPheromone()
    {
        if (pheromones == null || pheromones.Count() == 0)
        {
            return -1; // No pheromones found
        }

        // Set up variables to track the two closest pheromones and their distances
        int chosenPheromoneNum;
        float distanceToA = float.MaxValue;
        int pheromoneAIndex = -1;
        float distanceToB = float.MaxValue;
        int pheromoneBIndex = -1;

        for (int i = 0; i < pheromones.Count(); i++)
        {
            float tempDistance = (pheromones[i].transform.position - transform.position).magnitude;

            if (tempDistance < distanceToA)
            {
                distanceToB = distanceToA;
                pheromoneBIndex = pheromoneAIndex;
                distanceToA = tempDistance;
                pheromoneAIndex = i;
            }
            else if (tempDistance < distanceToB)
            {
                distanceToB = tempDistance;
                pheromoneBIndex = i;
            }
        }

        float distanceSum = distanceToA + distanceToB;
        float distanceThreshold = (distanceSum - distanceToA) / distanceSum;

        if (Random.value < distanceThreshold)
        {
            chosenPheromoneNum = pheromoneAIndex;
        }
        else
        {
            chosenPheromoneNum = pheromoneBIndex;
        }

        return chosenPheromoneNum;
    }

    public void MoveTowardsPheromone()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        // Jude code hoping to rotate ant mesh towards direction of movement
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += direction * Time.deltaTime * moveSpeed;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
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
        }
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
        Vector3 direction = (nest.transform.position - transform.position).normalized;
        transform.position += direction * Time.deltaTime * moveSpeed;

        transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, nest.transform.position) < 0.1f)
        {
            // Reached the nest
            Debug.Log("Returned to the nest");
            timeSinceLastFind = 0f;
            timeSinceLastSearch = 0f;
            previousPheromones.Clear();
            isReturningToNest = false;
            // Food += heldFood;
            heldFood = 0;
        }
    }

    public bool IsDead()
    {
        return !this.isAlive;
    }

    public void Kill()
    {
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
