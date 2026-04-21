using UnityEngine;
using FMODUnity;

public class AudioAnt : MonoBehaviour
{
    public EventReference AntDeathEvent; //Ant death sound FMOD asset
    public EventReference AntPickedUpFood; //Ant pickup food FMOD asset

    private AntController antController;

    void Awake()
    {
        antController = GetComponent<AntController>(); // get a ref to the owning ant controller

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
