using UnityEngine;
using FMODUnity;

public class AudioHUD : MonoBehaviour
{

    /// <summary>
    /// Sounds for different HUD alerts
    /// </summary>
    #region Alert Events
    public EventReference TutorialDingEvent;
    public EventReference DangerAlertEvent;
    public EventReference LowFoodAlertEvent;
    public EventReference AchievementAlertEvent;

    public void PlayTutorialDingAlert() => MusicManager.PlayOneShot(TutorialDingEvent);
    public void PlayDangerAlert() => MusicManager.PlayOneShot(DangerAlertEvent);
    public void PlayLowFoodAlert() => MusicManager.PlayOneShot(LowFoodAlertEvent);
    public void PlayAchievementAlert() => MusicManager.PlayOneShot(AchievementAlertEvent);
    #endregion

    /// <summary>
    /// Sounds for player interacting with spawnign different ant types
    /// </summary>
    #region Player Ant Spawning Events
    public EventReference ButtonHoverEvent;
    public EventReference ButtonSelectWorkerEvent;
    public EventReference ButtonSelectSoldierEvent;
    public EventReference ButtonSelectFireEvent;
    public EventReference ButtonSelectCarpenterEvent;

    public void PlayButtonHoverEvent() => MusicManager.PlayOneShot(ButtonHoverEvent);
    public void PlayButtonSelectWorkerEvent() => MusicManager.PlayOneShot(ButtonSelectWorkerEvent);
    public void PlayButtonSelectSoldierEvent() => MusicManager.PlayOneShot(ButtonSelectSoldierEvent);
    public void PlayButtonSelectFireEvent() => MusicManager.PlayOneShot(ButtonSelectFireEvent);
    public void PlayButtonSelectCarpenterEvent() => MusicManager.PlayOneShot(ButtonSelectCarpenterEvent);

    #endregion



    #region Special 1 Time Stingers
    public EventReference FirstAttackNotify;
    private bool bDidFirstAttackNotifyPlay = false;

    public EventReference FirstAntSpawnNotify;
    private bool bDidFirstAntSpawnNotifyPlay = false;

    public EventReference FirstCarpenterAntBuilding;
    private bool bDidFirstCarpenterAntBuildingPlay = false;

    public void PlayFirstAttackNotify()
    {
        if(bDidFirstAttackNotifyPlay == false)
        {
            bDidFirstAttackNotifyPlay = true;
            MusicManager.PlayOneShot(FirstAttackNotify);
        }
    }
    public void PlayFirstAntSpawnNotify()
    {
        if(bDidFirstAntSpawnNotifyPlay == false)
        {
            bDidFirstAntSpawnNotifyPlay = true;
            MusicManager.PlayOneShot(FirstAntSpawnNotify);
        }
    }
    public void PlayFirstCarpenterAntBuilding()
    {
        if(bDidFirstCarpenterAntBuildingPlay == false)
        {
            bDidFirstCarpenterAntBuildingPlay = true;
            MusicManager.PlayOneShot(FirstCarpenterAntBuilding);
        }
    }
    #endregion

}