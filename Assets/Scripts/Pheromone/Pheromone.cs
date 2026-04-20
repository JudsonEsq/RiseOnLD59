using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        Food,
        Danger,
        Attack,
        Gift,
        Hazard,
        None
    }

    public PheromoneType pheromoneType;
    public bool onCursor;

    public float GetDistanceToAnt(AntController ant)
    {
        return (transform.position - ant.transform.position).sqrMagnitude;
    }

    public bool IsOnCursor()
    {
        return onCursor;
    }
}
