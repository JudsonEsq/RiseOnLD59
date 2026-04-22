using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.InteropServices;


// How to subscribe to the beat
// MusicManager.OnMusicalBeat += YourFunctionHere;
// How to unsubscribe to the beat
// MusicManager.OnMusicalBeat -= YourFunctionHere;

// How to get BPM
// float bpm = MusicManager.instance.currentBPM;

// How to get time signature
// int timeSigUpper = MusicManager.instance.timeSignatureUpper;
// int timeSigLower = MusicManager.instance.timeSignatureLower;


public class MusicManager : MonoBehaviour
{
    #region Singleton
    public static MusicManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Channel
    public MusicChannel musicChannel;

    void OnEnable()
        {
            musicChannel.OnMusicCueRequested += HandleMusicRequest;
        }

    void OnDisable()
        {
            musicChannel.OnMusicCueRequested -= HandleMusicRequest;
        }
    #endregion

    #region Music Playback
    
    [Tooltip("Set this value and then right-click Music Manager COMPONENT to trigger")]
    public float TestPhaseValue = 0f;

    [ContextMenu("Test Music Phase")]
    public void TestMusicPhase() => SetGlobalParameter("MusicState", TestPhaseValue);


    public static event System.Action OnMusicalBeat;

    //volitile as multiple threads may use this varaible so don't cache it, use actual memory for latest value. 
    //This matters becaudse FMOD's beat callback fires on  separate audio thread, webGL will ignore this,
    //kept in due to builds on other platforms
    private volatile bool beatPending = false; 

    public float currentBPM = 120f; 
    public int timeSignatureUpper = 4;
    public int timeSignatureLower = 4;

    private EventInstance musicInstance;

    private bool isStartingMusic = false; //used to help prevent any doubling or race conditions if we get a raopid request to start music
    private void HandleMusicRequest(MusicCue cue)
    {
        if(!musicInstance.isValid() && !isStartingMusic)
        {
            isStartingMusic = true;
            StartCoroutine(StartMusicWhenReady(cue));
        }
        else if(musicInstance.isValid())
        {
            if(!string.IsNullOrEmpty(cue.musicStateParameter?.Name))
            {
                SetGlobalParameter(cue.musicStateParameter.Name, cue.musicStateParameter.Value);
            }
        }
    }

    private System.Collections.IEnumerator StartMusicWhenReady(MusicCue cue)
    {
        while (!RuntimeManager.HasBankLoaded("Music"))
            yield return null;

        musicInstance = RuntimeManager.CreateInstance(cue.musicEvent);
        musicInstance.start();
        isStartingMusic = false;
        if (!string.IsNullOrEmpty(cue.musicStateParameter?.Name))
            {
                SetGlobalParameter(cue.musicStateParameter.Name, cue.musicStateParameter.Value);
            }
        musicInstance.setCallback(BeatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
    }

    private void StopMusic()
    {      
        if(musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }

    void Update()
    {
        if(beatPending)
        {
            OnMusicalBeat?.Invoke();

            beatPending = false;
        }
    }

    //AOT ahead of time compilation
    //without this there is a chance compiler will strip or mangle the callback method as it looks "unused" from C# perspective
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private static FMOD.RESULT BeatCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr instancePtr, System.IntPtr parameters)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            instance.beatPending = true;
            var beatProperties = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
            
            MusicManager.instance.currentBPM = beatProperties.tempo;
            MusicManager.instance.timeSignatureUpper = beatProperties.timesignatureupper;
            MusicManager.instance.timeSignatureLower = beatProperties.timesignaturelower;
       
        }
        
        return FMOD.RESULT.OK;
    }

    

    void OnDestroy()
    {
        if(musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }

    #endregion


    #region Global Audio Control
        public static void PlayOneShot(EventReference eventRef)
        {
            if (eventRef.IsNull) return;
            RuntimeManager.PlayOneShot(eventRef);
        }

        public static void PlayOneShot(EventReference eventRef, Vector3 position)
        {
            if (eventRef.IsNull) return;
            RuntimeManager.PlayOneShot(eventRef, position);
        }

        public static void SetGlobalParameter(string ParameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(ParameterName, value);
        }


    #endregion


}
