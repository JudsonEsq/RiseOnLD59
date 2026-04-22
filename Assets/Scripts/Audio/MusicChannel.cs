using UnityEngine;

[CreateAssetMenu(fileName = "MusicChannel", menuName = "Scriptable Objects/MusicChannel")]

//Contains the "pipeline" or wire that messages are sent along from anyone needing to update the music state with a music cue
public class MusicChannel : ScriptableObject
{
    public event System.Action<MusicCue> OnMusicCueRequested;

    public void Raise(MusicCue cue)
    {
        OnMusicCueRequested?.Invoke(cue);
    }
    
}
