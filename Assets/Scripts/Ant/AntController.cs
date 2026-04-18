using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public List<Collider> pheromones; // List to store detected pheromones
    private List<Collider> previousPheromones = new List<Collider>(); // List to store previously detected pheromones to avoid re-targeting
    private Collider strongestPheromone; // Variable to store the strongest pheromone detected
    public float pheromoneRange = 10f; // Range within which the ant can detect pheromones
    public bool isMovingTowardsPheromone; // Flag to indicate if the ant is currently moving towards a pheromone
    private Vector3 targetPosition; // Variable to store the target position for movement
    public float searchTimer = 1f; // Time in seconds between pheromones searches
    public float searchDuration = 5f; // Time in seconds to search for new pheromones before returning to nest
    private float timeSinceLastSearch = 0f; // Time in seconds since the last pheromone search
    private float timeSinceLastFind = 0f; // Time in seconds since the last pheromone was found
    public GameObject nest; // Reference to the nest GameObject for returning after searching for pheromones
    public float moveSpeed = 2f; // Speed at which the ant moves towards pheromones and the nest

    void Start()
    {
        FindPheromones();
        isMovingTowardsPheromone = false;
    }

    void Update()
    {
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

        // Move towards the strongest pheromone
        if (!isMovingTowardsPheromone)
        {
            strongestPheromone = pheromones[PickStrongestPheromone()];
            isMovingTowardsPheromone = true;
            targetPosition = new Vector3 (strongestPheromone.transform.position.x, transform.position.y, strongestPheromone.transform.position.z);
            MoveTowardsPheromone();
        }
        
        else if (isMovingTowardsPheromone)
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
            if (previousPheromones.Contains(pheromones[i]))
            {
                pheromones.RemoveAt(i);
                i--;
            }
        }

        Debug.Log(pheromones.Count() + " pheromones found");
        return pheromones.Count() > 0 ? true : false;
    }

    // Method to pick the strongest pheromone based on proximity and type
    public int PickStrongestPheromone()
    {
        int strongestPheromoneNum = -1;
        float tempDistance;
        float minDistance = 0;

        for (int i = 0; i < pheromones.Count(); i++)
        {
            tempDistance = (pheromones[i].transform.position - transform.position).magnitude;

            if (minDistance == 0)
            {
                minDistance = tempDistance;
                strongestPheromoneNum = i;
            }
            else if (tempDistance < minDistance)
            {
                minDistance = tempDistance;
                strongestPheromoneNum = i;
            }
        }

        return strongestPheromoneNum;
    }

    public void MoveTowardsPheromone()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * Time.deltaTime * moveSpeed;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Reached the pheromone
            Debug.Log("Reached the pheromone");
            previousPheromones.Add(strongestPheromone);
            pheromones = null;
            strongestPheromone = null;
            isMovingTowardsPheromone = false;

            FindPheromones();
            timeSinceLastSearch = 0f;
        }
    }

    public void ReturnToNest()
    {
        Vector3 direction = (nest.transform.position - transform.position).normalized;
        transform.position += direction * Time.deltaTime * moveSpeed;

        if (Vector3.Distance(transform.position, nest.transform.position) < 0.1f)
        {
            // Reached the nest
            Debug.Log("Returned to the nest");
            timeSinceLastFind = 0f;
            timeSinceLastSearch = 0f;
            previousPheromones.Clear();
        }
    }
}
