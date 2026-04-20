using UnityEngine;

public class SoldierAntController : AntController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
        antType = AntType.Soldier;
        targetPheromones.Add(Pheromone.PheromoneType.Attack);
        targetPheromones.Add(Pheromone.PheromoneType.Good);
        targetPheromones.Add(Pheromone.PheromoneType.Food);
        avoidPheromones.Add(Pheromone.PheromoneType.Hazard);
    }

    // Update is called once per frame
    void Update()
    {
        Operate();
    }
}
