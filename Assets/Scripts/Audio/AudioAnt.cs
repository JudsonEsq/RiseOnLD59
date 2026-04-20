using UnityEngine;
using FMODUnity;

public class AudioAnt : MonoBehaviour
{
    public EventReference AntDeathEvent; //Ant death sound FMOD asset
    public EventReference AntPickedUpFood; //Ant pickup food FMOD asset

    public EventReference AntStarvationEvent;

    private AntController antController;

    void Awake()
    {
        antController = GetComponent<AntController>(); // get a ref to the owning ant controller

        antController.OnDeath += HandleDeath;
        antController.OnPickupFood += HandlePickupFood;
    }


    void HandleDeath()
    {
        MusicManager.PlayOneShot(AntDeathEvent);
    }

    void HandlePickupFood()
    {
        MusicManager.PlayOneShot(AntPickedUpFood);
    }

    void HandleAntStarvation()
    {
        MusicManager.PlayOneShot(AntStarvationEvent);
    }


    void OnDestroy()
    {
                antController.OnDeath -= HandleDeath;
        antController.OnPickupFood -= HandlePickupFood;
    }
}
