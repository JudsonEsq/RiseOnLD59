using NUnit.Framework;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine.UI;

public class ColonyController : MonoBehaviour
{
    [SerializeField]
    private AntManager antManager;

    [SerializeField]
    private FoodManager foodManager;

    [SerializeField]
    private int StartAntNum;

    [SerializeField]
    private List<GameObject> AntSpawnPoints = new List<GameObject>();

    [SerializeField]
    private float timeBetweenCulls = 10f;

    private float timeSinceLastCull = 0;

    [SerializeField]
    private Button SpawnWorkerButton;
    [SerializeField]
    private Button SpawnSoldierButton;
    [SerializeField]
    private Button SpawnFireButton;
    [SerializeField]
    private Button SpawnCarpenterButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < StartAntNum; i++)
        {
            SpawnAnt(AntController.AntType.Worker);
        }

        SpawnWorkerButton.onClick.AddListener(() => SpawnAnt(AntController.AntType.Worker));
        SpawnSoldierButton.onClick.AddListener(() => SpawnAnt(AntController.AntType.Soldier));
        SpawnFireButton.onClick.AddListener(() => SpawnAnt(AntController.AntType.Fire));
        SpawnCarpenterButton.onClick.AddListener(() => SpawnAnt(AntController.AntType.Carpenter));
    }

    void Update()
    {
        // Determine if we need to cull
        timeSinceLastCull += Time.deltaTime;
        if (timeSinceLastCull < timeBetweenCulls)
        {
            return;
        }
        timeSinceLastCull = -1;

        // find ants to kill
        int totalFoodCost = antManager.GetAntFoodCost();
        if (foodManager.CurrentFood() >= totalFoodCost)
        {
            // we have enough food to cover our current ants
            foodManager.SpendFood(totalFoodCost);
            return;
        }

        // We don't have enough food, need to kill some ants
        int debitFood = totalFoodCost - foodManager.CurrentFood();
        foodManager.SpendFood(totalFoodCost - debitFood); // Spend all the food

        antManager.CullAnts(debitFood);
    }

    public void SpawnAnt(AntController.AntType type)
    {
        int spawnIdx = Random.Range(0, AntSpawnPoints.Count);
        Transform antPoint = AntSpawnPoints[spawnIdx].transform;
        antManager.SpawnAnt(antPoint, type);
    }
}
