using UnityEngine;

public class MusicSelectTrack : MonoBehaviour
{
    public MusicCue musicCue;
    public MusicChannel musicChannel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicChannel.Raise(musicCue);
    }

}
