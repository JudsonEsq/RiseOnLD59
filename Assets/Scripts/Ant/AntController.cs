using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public List<Collider> pheromonesToTrack; // List to store detected pheromones
    public List<Collider> pheromonesToAvoid; // List to store detected pheromones
    public List<Collider> previousPheromones = new(); // List to store previously detected pheromones to avoid re-targeting
    public float pheromoneRange = 20f; // Range within which the ant can detect pheromones
    private GameObject targetObject; // Variable to store the target object for pathfinding
    private Vector3 targetPosition; // Variable to store the target position for movement
    public float searchTimer = 1f; // Time in seconds between pheromones searches
    public float searchDuration = 5f; // Time in seconds to search for new pheromones before returning to nest
    private float timeSinceLastFind = 0f; // Time in seconds since the last pheromone was found
    public GameObject nest; // Reference to the nest GameObject for returning after searching for pheromones
    public float moveSpeed = 2f; // Speed at which the ant moves towards pheromones and the nest
    public bool isReturningToNest = false; // Flag to indicate if the ant is currently returning to the nest
    public List<Pheromone.PheromoneType> targetPheromones = new();
    public List<Pheromone.PheromoneType> avoidPheromones = new();
    public float hazardDistance = 2f; // Distance within which the ant considers a pheromone to be a hazard
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


    /// <summary>
    /// Event Dispatchers
    /// </summary>
    
    public event Action OnDeath;
    public event Action OnPickupFood;
    private GameObject FoodModel;

    public void Init()
    {
		anim = GetComponent<Animator>();
		anim.SetBool("isMoving", false);
		anim.SetBool("isDead", false);
		
        isReturningToNest = false;
        targetObject = null;

        FoodModel = this.gameObject.transform.Find("Food").gameObject;
    }

    public void Operate()
    {
        if (IsDead()) return;

        if (isReturningToNest && targetObject != null)
        {
            if (ReachedTarget())
            {
                ReturnToNest();
            }

            else
            {
                MoveTowardsTarget();
            }

            return;
        }

        FindPheromones();

        if (DangerNearby())
        {
            ReturnToNest();
            return;
        }

        if (targetObject != null)
        {
            if (ReachedTarget())
            {
                Debug.Log("Reached target: " + targetObject.name);
                if (CheckForFood(targetObject))
                {
                    ReturnToNest();
                    return;
                } 

                if (targetObject.TryGetComponent(out Pheromone pheromone))
                {
                    pheromonesToTrack.Remove(pheromone.GetComponent<Collider>());
                    previousPheromones.Add(pheromone.GetComponent<Collider>());
                }

                targetObject = null;
            }
            else
            {
                MoveTowardsTarget();
                return;
            }
        }

        if (pheromonesToTrack.Count() > 0)
        {
            timeSinceLastFind = 0f;
            PickPheromone();
            MoveTowardsTarget();
            return;
        }
        else
        {
            timeSinceLastFind += Time.deltaTime;
        }

        if (timeSinceLastFind >= searchDuration)
        {
            ReturnToNest();
            return;
        }
    }

    // Method to check if the ant is dead
    public bool IsDead()
    {
        return !this.isAlive;
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
            if (pheromones[i].GetComponent<Pheromone>().pheromoneType == Pheromone.PheromoneType.None)
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

    // Method to check if there are any pheromones to avoid within a certain distance
    private bool DangerNearby()
    {
        for (int i = 0; i < pheromonesToAvoid.Count(); i++)
        {
            if (pheromonesToAvoid[i] != null)
            {
                if (pheromonesToAvoid[i].GetComponent<Pheromone>().GetDistanceToAnt(this) < hazardDistance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Method to set the ant's target position to the nest and return along the path of previously detected pheromones
    public void ReturnToNest()
    {
        if (ReachedTarget() && targetObject == nest)
        {
            Debug.Log("Returned to nest");
            targetObject = null;
            isReturningToNest = false;
            heldFood = 0;
            FoodModel.SetActive(false);
            return;
        }

        isReturningToNest = true;

        if (previousPheromones.Count() == 0 || previousPheromones == null)
        {
            Debug.Log("Targeting Nest");
            SetTarget(nest);
            return;
        }

        Collider nextPheromone = previousPheromones[previousPheromones.Count() - 1];
        Debug.Log("Returning to nest, next pheromone: " + nextPheromone.name);
        SetTarget(nextPheromone.gameObject);
        previousPheromones.RemoveAt(previousPheromones.Count() - 1);
    }

    // Method to set the ant's target object and position
    public void SetTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        targetObject = target;
        targetPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);

        // Also sets animation speed to match current music BPM
        float bpm = MusicManager.instance.currentBPM;

        // Animation by default is 24 steps per minute. We want 1 step per 4 beats.
        anim.speed = (bpm/48);
    }

    // Method to check if the ant has reached its target position
    public bool ReachedTarget()
    {
		anim.SetBool("isMoving", false);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            anim.speed = 1f;
            return true;
        }

        return false;
        
    }

    // Method to move the ant towards its target position
    public void MoveTowardsTarget()
    {
		anim.SetBool("isMoving", true);
		
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * direction;
		
		// Make ants face where they're going?
		Vector3 facing = transform.forward;
		if (facing != direction)
		{
			transform.forward = direction;
		}
    }

    // Method to pick the chosen pheromone based on proximity and type
    public void PickPheromone()
    {
        // Check is there are pheromonesToTrack
        if (pheromonesToTrack == null || pheromonesToTrack.Count() == 0)
        {
            return; // No pheromones found
        }

        if (pheromonesToTrack.Count() == 1)
        {
            SetTarget(pheromonesToTrack[0].gameObject);
            return;
        }

        Collider[] pheromonesArray = pheromonesToTrack.ToArray();
        Array.Sort(pheromonesArray, (a, b) => a.GetComponent<Pheromone>().GetDistanceToAnt(this).CompareTo(b.GetComponent<Pheromone>().GetDistanceToAnt(this)));
        int pheromonesCount = pheromonesArray.Length > maxPheromonesCount ? maxPheromonesCount : pheromonesArray.Length;

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

        float[] priorityThresholds = new float[distanceThresholds.Length];

        for (int i = 0; i < priorityThresholds.Length; i++)
        {
            priorityThresholds[i] = distanceThresholds[i] / thresholdSum;

            if (i > 0)
            {
                priorityThresholds[i] += priorityThresholds[i - 1];
            }
        }

        float tempValue = UnityEngine.Random.value;
        int chosenIndex = -1;

        for (int i = 0; i < priorityThresholds.Length; i++)
        {
            if (tempValue <= priorityThresholds[i])
            {
                chosenIndex = i;
                break;
            }
        }

        chosenIndex = (chosenIndex == -1) ? 0 : chosenIndex;
        SetTarget(pheromonesToTrack[chosenIndex].gameObject);
    }

    // Method to calculate the safety factor for a pheromone based on proximity to hazards
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

    // Method to check if the target object is a food source and pick up food if it is
    public bool CheckForFood(GameObject pheromone)
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
                return true;
            }
        }

        return false;
    }

    public void Kill()
    {
		anim.SetBool("isDead", true);
		
        Debug.Log("Killing Ant");
        this.isAlive = false;
        OnDeath?.Invoke();
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
        OnPickupFood?.Invoke();
        FoodModel.SetActive(true);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Debug.Log("Collided with obstacle, returning to nest");
            ReturnToNest();
        }
    }
}
