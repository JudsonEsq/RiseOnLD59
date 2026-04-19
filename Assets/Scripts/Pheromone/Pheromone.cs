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

    public float GetDistanceToAnt(AntController ant)
    {
        return (transform.position - ant.transform.position).sqrMagnitude;
    }
}
