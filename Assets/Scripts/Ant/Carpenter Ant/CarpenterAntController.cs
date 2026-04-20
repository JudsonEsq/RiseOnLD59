using UnityEngine;

public class CarpenterAntController : AntController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
        antType = AntType.Carpenter;
        targetPheromones.Add(Pheromone.PheromoneType.Hazard);
        targetPheromones.Add(Pheromone.PheromoneType.Food);
        avoidPheromones.Add(Pheromone.PheromoneType.Danger);
    }

    // Update is called once per frame
    void Update()
    {
        Operate();
    }
}
