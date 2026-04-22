using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "MusicCue", menuName = "Scriptable Objects/MusicCue")]

//Holds information of what musical cue to play and then is used by the music system when called to update the FMOD status
public class MusicCue : ScriptableObject
{
    public EventReference musicEvent;
    public ParamRef musicStateParameter;
}
