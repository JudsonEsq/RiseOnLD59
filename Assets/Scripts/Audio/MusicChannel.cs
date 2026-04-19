using UnityEngine;

[CreateAssetMenu(fileName = "MusicChannel", menuName = "Scriptable Objects/MusicChannel")]
public class MusicChannel : ScriptableObject
{
    public event System.Action<MusicCue> OnMusicCueRequested;

    public void Raise(MusicCue cue)
    {
        OnMusicCueRequested?.Invoke(cue);
    }
    
}
