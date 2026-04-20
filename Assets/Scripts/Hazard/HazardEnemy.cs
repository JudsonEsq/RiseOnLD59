using FMODUnity;
using UnityEngine;

public class HazardEnemy : Hazard
{
    public override void DisableHazardSpecific()
    {
        Transform antlion = this.gameObject.transform.Find("Antlion Animated");
        if (antlion != null)
        {
            antlion.gameObject.SetActive(false);
        }
        this.gameObject.GetComponent<BoxCollider>().enabled = false;

        StudioEventEmitter emitter = this.gameObject.GetComponent<StudioEventEmitter>();
        if (emitter != null)
        {
            emitter.Stop();
        }
    }
}
