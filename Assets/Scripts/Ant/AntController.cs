using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    [Header("General Ant Settings")]
    public bool isAlive = true;
    [SerializeField] private int heldFood = 0;
    public AntType antType;
    [SerializeField] private int foodCost = 1;
    [Serializable] public enum AntType
    {
        Worker,
        Soldier,
        Fire,
        Carpenter,
    }

    [Header("Pheromone Detection")]
    public List<Pheromone.PheromoneType> targetPheromones = new();
    public List<Pheromone.PheromoneType> avoidPheromones = new();
    public List<Collider> pheromonesToTrack; // List to store detected pheromones
    public List<Collider> pheromonesToAvoid; // List to store detected pheromones
    public List<Collider> previousPheromones = new(); // List to store previously detected pheromones to avoid re-targeting
    public float pheromoneRange = 20f; // Range within which the ant can detect pheromones
    [SerializeField] private int maxPheromonesCount = 3; // Maximum number of pheromones to consider when picking a target
    [SerializeField] private float hazardDistance = 2f; // Distance within which the ant considers a pheromone to be a hazard

    [Header("Movement and Targeting")]
    private GameObject targetObject; // Variable to store the target object for pathfinding
    private Vector3 targetPosition; // Variable to store the target position for movement
    public float searchDuration = 5f; // Time in seconds to search for new pheromones before returning to nest
    private float timeSinceLastFind = 0f; // Time in seconds since the last pheromone was found
    public GameObject nest; // Reference to the nest GameObject for returning after searching for pheromones
    public float moveSpeed = 2f; // Speed at which the ant moves towards pheromones and the nest
    public bool isReturningToNest = false; // Flag to indicate if the ant is currently returning to the nest

    [Header("Idle Settings")]
    public bool waitingForFood = false; // Flag to indicate if the ant is waiting for a food source to become available for harvest
    [SerializeField] private float idleTimer = 0f; // Timer to track how long the ant has been waiting for a food source
    [SerializeField] private float idleDuration = 5f; // Time in seconds to wait for a food source to become available for harvest before returning to the nest

    private Animator anim;

    /// <summary>
    /// Event Dispatchers
    /// </summary>

    public event Action OnDeath;
    public event Action OnPickupFood;
    private GameObject FoodModel;

    [Header("Music Timing Settings")]
    public float walkSpeedMultiplier = 1;
    public float idleSpeedMultiplier = 1;

    public void Init()
    {
        anim = TryGetComponent(out Animator animator) ? animator : null;

        if (anim != null)
        {
            anim.SetBool("isMoving", false);
            anim.SetBool("isDead", false);
        }

        isReturningToNest = false;
        targetObject = null;

        Transform transform = this.gameObject.transform.Find("Food");
        if (transform != null)
        {
            FoodModel = this.gameObject.transform.Find("Food").gameObject;
        }

        MusicManager.OnMusicalBeat += onDownbeat;
    }

    void OnEnable()
    {
        MusicManager.OnMusicalBeat += SyncAnimationsToMusic;
    }

    public void Operate()
    {

        if (IsDead()) return;

        if (waitingForFood)
        {
            if (CheckForFood(targetObject))
            {
                ReturnToNest();
            }
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                waitingForFood = false;
                idleTimer = 0f;
                ReturnToNest();
            }
            return;
        }

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

                if (waitingForFood)
                {
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
    private void FindPheromones()
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

            // Filter out Pheromones that are on the cursor
            if (pheromones[i].GetComponent<Pheromone>().IsOnCursor())
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
    private void ReturnToNest()
    {
        if (ReachedTarget() && targetObject == nest)
        {
            Debug.Log("Returned to nest");
            targetObject = null;
            isReturningToNest = false;
            nest.GetComponent<FoodManager>().AddFood(heldFood);
            heldFood = 0;
            if (FoodModel != null)
            {
                FoodModel.SetActive(false);
            }
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
    private void SetTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        targetObject = target;
        targetPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);

    }

    // Method to check if the ant has reached its target position
    private bool ReachedTarget()
    {
        if (anim != null)
        {
            anim.SetBool("isMoving", false);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (anim != null)
            {
                anim.speed = 1f;
            }
            return true;
        }

        return false;
        
    }

    float boogieDuration = 0f;
    void onDownbeat()
    {
        boogieDuration = 0.1f;
    }

    // Method to move the ant towards its target position
    private void MoveTowardsTarget()
    {
		if (anim != null)
        {
            anim.SetBool("isMoving", true);
        }

        if(boogieDuration > 0f)
        {
            boogieDuration -= Time.deltaTime;
            if(anim!= null) anim.SetBool("isMoving", false);
            return;
        }


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
    private void PickPheromone()
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
    private bool CheckForFood(GameObject pheromone)
    {
        if (pheromone.TryGetComponent(out FoodSource foodSource))
        {
            int foodToPickup = foodSource.RequestHarvest(this);

            if (foodToPickup > 0)
            {
                PickupFood(foodToPickup);
                return true;
            }
        }

        return false;
    }

    public void Kill()
    {
        if (anim != null)
        {
            anim.SetBool("isDead", true);
        }
		
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

    private void PickupFood(int foodAmount)
    {
        heldFood = foodAmount;
        OnPickupFood?.Invoke();
        if (FoodModel != null)
        {
            FoodModel.SetActive(true);
        }
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

    //Every beat update from FMOD will upate the ant animation cycle speed so that any animation that is playing will stay in sync with the music, regardless of animation length.
    private void SyncAnimationsToMusic()
    {
        if(anim == null) return;
        float clipDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // By using the time signature so that we have the full bar duration in combination with the tempo
        float barDuration = MusicManager.instance.timeSignatureUpper * (60/MusicManager.instance.currentBPM) * (4f / MusicManager.instance.timeSignatureLower);
        float multiplier  = anim.GetBool("isMoving") ? walkSpeedMultiplier : idleSpeedMultiplier;
        anim.speed = clipDuration/barDuration * multiplier;
    }

    void OnDisable()
    {
        MusicManager.OnMusicalBeat -= SyncAnimationsToMusic;
    }

}
