using UnityEngine;
using FMODUnity;

public class AudioHUD : MonoBehaviour
{
    public EventReference TutorialDingEvent;
    public EventReference DangerAlertEvent;
    public EventReference LowFoodAlertEvent;
    public EventReference AchievementAlertEvent;

    public void PlayTutorialDingAlert() => MusicManager.PlayOneShot(TutorialDingEvent);
    public void PlayDangerAlert() => MusicManager.PlayOneShot(DangerAlertEvent);
    public void PlayLowFoodAlert() => MusicManager.PlayOneShot(LowFoodAlertEvent);
    public void PlayAchievementAlert() => MusicManager.PlayOneShot(AchievementAlertEvent);

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