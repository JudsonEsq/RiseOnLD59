using UnityEngine;
using FMODUnity;

abstract public class Hazard : MonoBehaviour
{
    public abstract void DisableHazardSpecific();

    public EventReference PlayEnemyDefeated;
    public EventReference PlayTrapDefeated;
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

    private bool IsEnabled = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Ant")
        {
            //Debug.Log("Ant On the Hazard");
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
                //Debug.Log("Hazard Cleared!");
                DisableHazard();
            }
        }
    }

    private bool doesKillAnt(AntController ant)
    {
        if (!IsEnabled)
        {
            return false;
        }

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

    public void DisableHazard()
    {
        this.IsEnabled = false;
        GetComponent<Collider>().enabled = false;
        DisableHazardSpecific();
        switch(type)
        {
            case HazardType.Enemy:
            MusicManager.PlayOneShot(PlayEnemyDefeated, transform.position);
            break;
            case HazardType.Spike:
            MusicManager.PlayOneShot(PlayTrapDefeated, transform.position);
            break;
            case HazardType.Fire:
            MusicManager.PlayOneShot(PlayTrapDefeated, transform.position);
            break;
            default:
            MusicManager.PlayOneShot(PlayTrapDefeated, transform.position);
            break;
        }
    }
}
