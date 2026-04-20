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

    [SerializeField]
    private int numAntInteractionsToClear = 5;

    private int numAntInteractions = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Ant")
        {
            AntController ant = other.gameObject.GetComponent<AntController>();
            if (doesKillAnt(ant))
            {
                ant.Kill();
            } else
            {
                numAntInteractions++;
            }

            if (numAntInteractions >= numAntInteractionsToClear)
            {
                Debug.Log("Hazard Cleared!");
                Destroy(this.gameObject);
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
