using UnityEngine;

public class AudioSceneSetup : MonoBehaviour
{
    public MusicChannel musicChannel;
    public MusicCue musicCue;
    void Start()
    {
        musicChannel.Raise(musicCue);
    }
}
