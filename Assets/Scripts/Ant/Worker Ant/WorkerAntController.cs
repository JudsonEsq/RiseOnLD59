using UnityEngine;

public class WorkerAntController : AntController
{
    void Start()
    {
        Init();
        antType = AntType.Worker;
        targetPheromones.Add(Pheromone.PheromoneType.Food);
        avoidPheromones.Add(Pheromone.PheromoneType.Danger);
        avoidPheromones.Add(Pheromone.PheromoneType.Hazard);
    }

    void Update()
    {
        Operate();
    }
}
