using FMODUnity;
using UnityEngine;


//Attach to player character and play out different click types
public class AudioPlayer : MonoBehaviour
{
    public EventReference PheromoneGuideEvent;
    public EventReference PheromoneHazardEvent;
    public EventReference PheromoneAttackEvent;
    public EventReference PheromoneBuildEvent;

    public void PlayPheromoneGuide() => MusicManager.PlayOneShot(PheromoneGuideEvent);
    public void PlayPheromoneHazard() => MusicManager.PlayOneShot(PheromoneHazardEvent);
    public void PlayPheromoneAttack() => MusicManager.PlayOneShot(PheromoneAttackEvent);
    public void PlayPheromoneBuild() => MusicManager.PlayOneShot(PheromoneBuildEvent);

}
