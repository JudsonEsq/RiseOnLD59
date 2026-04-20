using System.Collections;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    [SerializeField]
    private UnityEngine.UI.Image curtain;

    [SerializeField]
    private float FadeSpeed = 50f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnGameExitPress() 
   {
      Application.Quit();
   }

   public void OnGameRestartPress()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

    IEnumerator FadeInCurtain(string target)
    {
        Color newCol = curtain.color;
        while (curtain.color.a < .99)
        {
            newCol.a += FadeSpeed * Time.deltaTime;
            curtain.color = newCol;
            yield return null;
        }

    }

    IEnumerator FadeInCurtain()
    {
        Color newCol = curtain.color;
        while (curtain.color.a < .99)
        {
            newCol.a += FadeSpeed * Time.deltaTime;
            curtain.color = newCol;
            yield return null;
        }

        Time.timeScale = 0f;

    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCurtain());
    }

   public void LoadScene(string target)
   {
        StartCoroutine(FadeInCurtain(target));
        
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
