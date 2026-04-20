using UnityEngine;
using FMODUnity;

public class AudioHUD : MonoBehaviour
{
    #region Pheromone Events
    [Header("Pheromone Events")]
    public EventReference ButtonFoodSelect;
    public EventReference ButtonDangerSelect;
    public EventReference ButtonAttackSelect;
    public EventReference ButtonGiftSelect;

    public EventReference ButtonFoodPlace;
    public EventReference ButtonDangerPlace;
    public EventReference ButtonAttackPlace;
    public EventReference ButtonGiftPlace;
    
    public EventReference ButtonHoverEvent;
    public EventReference PlaceItemEvent;

    //play when player selects the button the UI
    public void PlayButtonFoodSelect() => MusicManager.PlayOneShot(ButtonFoodSelect);
    public void PlayButtonDangerSelect() => MusicManager.PlayOneShot(ButtonDangerSelect);
    public void PlayButtonAttackSelect() => MusicManager.PlayOneShot(ButtonAttackSelect);
    public void PlayButtonGiftSelect() => MusicManager.PlayOneShot(ButtonGiftSelect);

    public void PlayButtonHoverEvent() => MusicManager.PlayOneShot(ButtonHoverEvent);
    public void PlayPlaceItemEvent() => MusicManager.PlayOneShot(PlaceItemEvent);


    public void PlayPheromonePlacement(Pheromone.PheromoneType type)
    {
        switch(type)
        {
            case Pheromone.PheromoneType.Food:
            MusicManager.PlayOneShot(ButtonFoodPlace);
            break;
            case Pheromone.PheromoneType.Attack:
            MusicManager.PlayOneShot(ButtonAttackPlace);
            break;
            case Pheromone.PheromoneType.Danger:
            MusicManager.PlayOneShot(ButtonDangerPlace);
            break;
            case Pheromone.PheromoneType.Gift:
            MusicManager.PlayOneShot(ButtonGiftPlace);
            break;
            default:
            MusicManager.PlayOneShot(PlaceItemEvent);
            break;
        }
    }
    #endregion
    
    
    /// <summary>
    /// Sounds for different HUD alerts
    /// </summary>
    #region Alert Events
    [Header("Alert Events")]
    public EventReference TutorialDingEvent;
    public EventReference DangerAlertEvent;
    public EventReference LowFoodAlertEvent;
    public EventReference AchievementAlertEvent;

    public EventReference AntStarvationEvent;

    public void PlayTutorialDingAlert() => MusicManager.PlayOneShot(TutorialDingEvent);
    public void PlayDangerAlert() => MusicManager.PlayOneShot(DangerAlertEvent);
    public void PlayLowFoodAlert() => MusicManager.PlayOneShot(LowFoodAlertEvent);
    public void PlayAchievementAlert() => MusicManager.PlayOneShot(AchievementAlertEvent);
    public void PlayAntStarvatationEvent() => MusicManager.PlayOneShot(AntStarvationEvent);
    
    #endregion
    
    /// <summary>
    /// Sounds for player interacting with spawnign different ant types
    /// </summary>
    #region Player Ant Spawning Events
    [Header("Player Spawning Ants Events")]

    public EventReference ButtonSelectWorkerEvent;
    public EventReference ButtonSelectSoldierEvent;
    public EventReference ButtonSelectCarpenterEvent;


    public void PlayButtonSelectWorkerEvent() => MusicManager.PlayOneShot(ButtonSelectWorkerEvent);
    public void PlayButtonSelectSoldierEvent() => MusicManager.PlayOneShot(ButtonSelectSoldierEvent);
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


    //antmananger.callants

    public AntManager antManagerInstance;

    

}