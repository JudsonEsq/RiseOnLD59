using UnityEngine;

public class WorkerAntController : AntController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        antType = AntType.Worker;
        targetPheromones.Add(Pheromone.PheromoneType.Food);
        avoidPheromones.Add(Pheromone.PheromoneType.Danger);
        avoidPheromones.Add(Pheromone.PheromoneType.Hazard);
        FindPheromones();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
