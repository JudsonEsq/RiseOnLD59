using UnityEngine;

public class FoodSource : MonoBehaviour
{
    [Header("Food Source Settings")]
    [SerializeField] private int foodValue = 5; // Amount of food available per harvest
    [SerializeField] private int foodCapacity = 100; // Maximum amount of food this source can hold
    [SerializeField] private int currentFood; // Current amount of food available
    [SerializeField] private bool isDepletable = true; // Flag to indicate if the food source can be depleted
    public bool isDepleted = false; // Flag to indicate if the food source is depleted

    [Header("Harvest Settings")]
    [SerializeField] private int harvestCounter = 0; // Counter to track how many times this food source has been harvested
    [SerializeField] private int allowableHarvests = 5; // Maximum number of times this food source can be harvested before it becomes unavailable
    [SerializeField] private float harvestCooldown = 1f; // Time in seconds between harvests
    [SerializeField] private bool acceptingHarvest = true; // Flag to indicate if the food source is currently accepting harvest requests
    private float harvestTimer = 0f;

    void Start()
    {
        currentFood = foodCapacity;
    }

    void Update()
    {
        if (GetComponent<Pheromone>().pheromoneType != Pheromone.PheromoneType.Food)
        {
            return;
        }

        if (isDepletable)
        {
            isDepleted = currentFood <= 0;
        }

        if (isDepleted)
        {
            GetComponent<Pheromone>().pheromoneType = Pheromone.PheromoneType.None;
            acceptingHarvest = false;
        }

        acceptingHarvest = harvestCounter < allowableHarvests;

        if (!acceptingHarvest)
        {
            harvestTimer += Time.deltaTime;
            if (harvestTimer >= harvestCooldown)
            {
                harvestCounter--;
                harvestTimer = 0f;
            }
            return;
        }
    }

    /// <summary>
    /// Handles harvest requests from ants. Returns amount of food harvested.
    /// </summary>
    /// <param name="ant"></param>
    /// <returns></returns>
    public int RequestHarvest(AntController ant)
    {
        if (!acceptingHarvest)
        {
            Debug.Log("Food source is currently unavailable for harvest, waiting...");
            ant.waitingForFood = true;
            return 0;
        }

        if (currentFood > 0)
        {
            int harvestAmount = Mathf.Min(foodValue, currentFood);
            if (isDepletable)
            {
                currentFood -= harvestAmount;
                isDepleted = currentFood <= 0;
            }

            ant.waitingForFood = false;
            harvestCounter++;
            acceptingHarvest = harvestCounter < allowableHarvests;
            return harvestAmount;
        }
        else
        {
            Debug.Log("Food source is depleted.");
            return 0;
        }
    }
}
