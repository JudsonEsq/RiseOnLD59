using UnityEngine;
using FMODUnity;
using FMOD.Studio;

//How to subscribe to the beat
// MusicManager.OnMusicalBeat += YourFunctionHere;
// How to unsubscribe to the beat
// MusicManager.OnMusicalBeat -= YourFunctionHere;




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
}
