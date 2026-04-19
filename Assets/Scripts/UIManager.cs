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
}
