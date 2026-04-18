using UnityEngine;

public class AudioSceneSetup : MonoBehaviour
{

    public MusicChannel musicChannel;
    public MusicCue musicCue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicChannel.Raise(musicCue);
    }
}
