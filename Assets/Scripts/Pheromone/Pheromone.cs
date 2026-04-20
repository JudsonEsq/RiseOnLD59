using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        Food,
        Good,
        Attack,
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

    void Start()
    {
        // Access the renderer and change the material color
        Renderer rend = GetComponentInChildren<Renderer>();
        if (pheromoneType == PheromoneType.Good)
        {
            rend.material.color = Color.green;
        }
        else if (pheromoneType == PheromoneType.Attack)
        {
            rend.material.color = Color.red;
        }
        else if (pheromoneType == PheromoneType.Hazard)
        {
            rend.material.color = Color.orange;
        }
    }
}
