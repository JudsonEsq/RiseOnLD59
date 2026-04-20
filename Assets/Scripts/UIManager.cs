using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
   public void OnGameExitPress() 
   {
      Application.Quit();
   }

   public void OnGameRestartPress()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   public void OnPausePress() {
      
   }

   public void OnResumePress() {
      
   } 

   public void OnMisophoniaFilterToggle(bool value)
   {
      Debug.Log("Toggle is now: " + value);
   }
   
   public void OnMainVolChanged(float value)
    {
        Debug.Log("New MainVol: " + value);
    }

    public void OnSFXVolChanged(float value)
    {
        Debug.Log("New SFXVol: " + value);
    }

    public void OnMusicVolChanged(float value)
    {
        Debug.Log("New MusicVol: " + value);
    }
   }
