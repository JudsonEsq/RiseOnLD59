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
            Destroy(this);
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
    

    public static event System.Action OnMusicalBeat;

    private volatile bool beatPending = false;

    public float currentBPM = 120f; // Default BPM, can be updated by beat callback
    public int timeSignatureUpper = 4; // Default time signature upper value, can be updated by beat callback
    public int timeSignatureLower = 4; // Default time signature lower value, can be updated by beat callback

    private EventInstance musicInstance;
    private void HandleMusicRequest(MusicCue cue)
    {
        //if we dont have the instance yet, lets create one
        if(!musicInstance.isValid())
        {
            musicInstance = RuntimeManager.CreateInstance(cue.musicEvent);
            if(!cue.musicStateParameter.Name.Equals(string.Empty))
            {
                musicInstance.setParameterByID(cue.musicStateParameter.ID, cue.musicStateParameter.Value);
            }
            //start the music
            musicInstance.start();
            musicInstance.setCallback(BeatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        }
        else
        {
            //set the parameter if we have one
            if(!cue.musicStateParameter.Name.Equals(string.Empty))
            {
                musicInstance.setParameterByID(cue.musicStateParameter.ID, cue.musicStateParameter.Value);
            }
        }
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


    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private static FMOD.RESULT BeatCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr instancePtr, System.IntPtr parameters)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            instance.beatPending = true;
            var beatProperties = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
            //Debug.Log($"Beat! BPM: {beatProperties.bpm}, Position: {beatProperties.position}, Bar: {beatProperties.bar}, Beat: {beatProperties.beat}, Tick: {beatProperties.tick}");
       
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

        public static void SetGlobalParameter(string parameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }


    #endregion


}
