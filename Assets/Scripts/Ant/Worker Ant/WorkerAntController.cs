using UnityEngine;

public class WorkerAntController : AntController
{
    void Start()
    {
        Init();
        antType = AntType.Worker;
        targetPheromones.Add(Pheromone.PheromoneType.Good);
        targetPheromones.Add(Pheromone.PheromoneType.Food);
        avoidPheromones.Add(Pheromone.PheromoneType.Attack);
        avoidPheromones.Add(Pheromone.PheromoneType.Hazard);
    }

    void Update()
    {
        Operate();
    }
}
