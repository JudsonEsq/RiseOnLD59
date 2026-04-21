using UnityEngine;
using FMODUnity;
using FMOD.Studio;

using UnityEngine.UI;


public class AudioMainMenu : MonoBehaviour
{
    [Header("Events")]
    public EventReference hoverEvent;
    public EventReference clickEvent;
    public EventReference backEvent;
    public EventReference sliderTickEvent;

    public EventReference StartGameEvent;

    public MusicChannel musicChannel;
    public MusicCue musicMenuCue;

    public MusicCue musicGameplayCue;

    public void PlayHover() => PlayEvent(hoverEvent);
    public void PlayClick() => PlayEvent(clickEvent);
    public void PlayBack() => PlayEvent(backEvent);
    public void PlaySliderTick() => PlayEvent(sliderTickEvent);

    public void PlayStartGameSound() => PlayEvent(StartGameEvent);

    private EventInstance misophoniaSnapshot;

    // Slider Data Structure
    [System.Serializable]
    public class VCASlider
    {
        public Slider slider;
        public string vcaName;
    };

    public VCASlider[] vcaSliders;

    void Start()
    {
        Invoke("InitVCASliders", 0.1f); // Delay to ensure sliders are initialized
        musicChannel.Raise(musicMenuCue);
        SetRunInBackground(true); //dev set as we probably wont run into this issue in web build
        misophoniaSnapshot = RuntimeManager.CreateInstance("snapshot:/MisophoniaFilter");
    }

    public void SetRunInBackground(bool runInBackground)
    {
        Application.runInBackground = runInBackground;
        RuntimeManager.PauseAllEvents(!runInBackground);
    }

    // SLIDERS //////////
    void InitVCASliders()
    {
        foreach(var vcaSlider in vcaSliders)
        {
            if(vcaSlider.slider != null && !string.IsNullOrEmpty(vcaSlider.vcaName))
            {
                vcaSlider.slider.onValueChanged.AddListener((value) => SetVCAVolume(vcaSlider.vcaName, value));
            
                VCA vca;
                if(TryGetVCA(vcaSlider.vcaName, out vca))
                {
                    float currentVolume;
                    vca.getVolume(out currentVolume);
                    vcaSlider.slider.value = currentVolume;
                }
            }
        }
    }

    private void PlayEvent(EventReference eventRef)
    {
        if (eventRef.IsNull)return;

        RuntimeManager.PlayOneShot(eventRef);
    }

    public void SetVCAVolume(string vcaName, float value)
    {
        VCA vca;
        if(TryGetVCA(vcaName, out vca))
        {
            vca.setVolume(value);
        }
    }

    private bool TryGetVCA(string vcaName, out VCA vca)
    {
        vca = RuntimeManager.GetVCA("vca:/" + vcaName);
        if(vca.isValid())
        {
            return true;
        }
        Debug.LogWarning("VCA not found: " + vcaName);
        return false;
    }

    public void MisophoniaToggle(bool bEnableMisophonia)
    {
        if(bEnableMisophonia)
            misophoniaSnapshot.start();
        else
        misophoniaSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    //Used to play next scene from main menu
    public void PlayTransitionFromMenuToGameplay()
    {
        Debug.Log("PlayTransition");
        musicChannel.Raise(musicGameplayCue);
    }

    void OnDestroy()
    {
        misophoniaSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        misophoniaSnapshot.release();
    }
}



