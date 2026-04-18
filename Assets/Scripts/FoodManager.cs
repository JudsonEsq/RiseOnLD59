using TMPro;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    private int TotalFood;
    private float timeSinceFoodSpawn = 0;

    [SerializeField]
    private TMP_Text FoodTracker;


    [SerializeField]
    private int FoodSpawnTimeSeconds = 10;

    [SerializeField]
    private int FoodSpawnAmount = 10;

    // Update is called once per frame
    void Update()
    {
        FoodTracker.text = TotalFood.ToString();
        timeSinceFoodSpawn += Time.deltaTime;
        if (timeSinceFoodSpawn > FoodSpawnTimeSeconds)
        {
            AddFood(FoodSpawnAmount);
            timeSinceFoodSpawn = 0;
        }
    }

    public void AddFood(int food)
    {
        TotalFood += food;
    }

    public void SpendFood(int food)
    {
        TotalFood -= food;
    }
}
