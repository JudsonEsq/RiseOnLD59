using UnityEngine;

public class FoodSource : MonoBehaviour
{
    public int foodValue = 5;
    public int foodCapacity = 100;
    public int currentFood;
    public bool isDepleted = false;
    public bool isDepletable = true;

    void Start()
    {
        currentFood = foodCapacity;
    }

    void Update()
    {
        if (!TryGetComponent(out Pheromone pheromone))
        {
            return;
        }

        if (isDepletable)
        {
            isDepleted = currentFood <= 0;
        }

        if (isDepleted)
        {
            Destroy(GetComponent<Pheromone>());
        }
    }
}
