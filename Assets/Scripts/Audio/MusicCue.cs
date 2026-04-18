using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "MusicCue", menuName = "Scriptable Objects/MusicCue")]
public class MusicCue : ScriptableObject
{
    public EventReference musicEvent;
    public ParamRef musicStateParameter;
}
