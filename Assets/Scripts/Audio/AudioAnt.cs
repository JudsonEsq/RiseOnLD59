using UnityEngine;
using FMODUnity;

public class AudioAnt : MonoBehaviour
{
    public EventReference AntDeathEvent;
    public EventReference AntPickedUpFood;

    private AntController antController;

    void Awake()
    {
        antController = GetComponent<AntController>();

        antController.OnDeath += HandleDeath;
        antController.OnPickupFood += HandlePickupFood;
    }

    void HandleDeath()
    {
        MusicManager.PlayOneShot(AntDeathEvent, transform.position);
    }

    void HandlePickupFood()
    {
        MusicManager.PlayOneShot(AntPickedUpFood, transform.position);
    }

    void OnDestroy()
    {
                antController.OnDeath -= HandleDeath;
        antController.OnPickupFood -= HandlePickupFood;
    }
}
