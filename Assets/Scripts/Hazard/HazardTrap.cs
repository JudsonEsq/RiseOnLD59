using UnityEngine;

public class HazardTrap : Hazard
{
    public override void DisableHazardSpecific()
    {
        Transform blockers = this.gameObject.transform.Find("Blockers");
        if (blockers != null)
        {
            blockers.gameObject.SetActive(true);
        }
    }
}
