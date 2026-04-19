using UnityEngine;

public class Hazard : MonoBehaviour
{
    public enum HazardType
    {
        Spike,
        Fire,
        Enemy,
    }

    public HazardType type;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hazard Collision");
        if (other.GetComponent<Collider>().tag == "Ant")
        {
            Debug.Log("Ant on the Hazard!");
            AntController ant = other.gameObject.GetComponent<AntController>();
            if (doesKillAnt(ant))
            {
                ant.Kill();
            }
        }
    }

    private bool doesKillAnt(AntController ant)
    {
        if (ant.antType == AntController.AntType.Fire && type == HazardType.Fire)
        {
            return false;
        }

        if (ant.antType == AntController.AntType.Soldier && type == HazardType.Enemy)
        {
            return false;
        }

        if (ant.antType == AntController.AntType.Carpenter && type == HazardType.Spike)
        {
            return false;
        }

        return true;
    }
}
