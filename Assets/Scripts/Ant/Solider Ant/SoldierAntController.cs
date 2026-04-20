using UnityEngine;

public class SoldierAntController : AntController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
        antType = AntType.Soldier;
        targetPheromones.Add(Pheromone.PheromoneType.Danger);
        targetPheromones.Add(Pheromone.PheromoneType.Food);
    }

    // Update is called once per frame
    void Update()
    {
        Operate();
    }
}
