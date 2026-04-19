using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        None,
        Food,
        Danger,
        Attack,
        Gift,
        Hazard
    }

    public PheromoneType pheromoneType;
}
