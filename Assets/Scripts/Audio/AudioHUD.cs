using UnityEngine;
using FMODUnity;

public class AudioHUD : MonoBehaviour
{
    public EventReference DangerAlertEvent;
    public EventReference LowFoodAlertEvent;
    public EventReference AchievementAlertEvent;

    public void PlayDangerAlert() => MusicManager.PlayOneShot(DangerAlertEvent);
    public void PlayLowFoodAlert() => MusicManager.PlayOneShot(LowFoodAlertEvent);
    public void PlayAchievementAlert() => MusicManager.PlayOneShot(AchievementAlertEvent);

}
