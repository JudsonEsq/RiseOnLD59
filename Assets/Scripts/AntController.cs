using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public List<Collider> pheromones;
    private List<Collider> previousPheromones;
    private Collider strongestPheromone;
    public float pheromoneRange;
    public bool isMovingTowardsPheromone;
    private Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pheromoneRange = 10f;
        isMovingTowardsPheromone = false;
    }

    // Update is called once per frame
    void Update()
    {
        FindPheromones();
        
        if (pheromones.Count() == 0)
        {
            // No pheromones found, stay in position
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

    public void FindPheromones()
    {
        pheromones = Physics.OverlapSphere(transform.position, pheromoneRange, LayerMask.GetMask("Pheromones")).ToList();
        Debug.Log(pheromones.Count() + " pheromones found");
    }

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
        transform.position += direction * Time.deltaTime * 2f; // Move speed of 2 units per second

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Reached the pheromone
            pheromones = null;
            strongestPheromone = null;
            isMovingTowardsPheromone = false;
        }
    }
}
