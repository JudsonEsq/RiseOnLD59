using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        Food,
        Danger,
        Attack,
        Gift,
        Hazard
    }

    public PheromoneType pheromoneType;
}
