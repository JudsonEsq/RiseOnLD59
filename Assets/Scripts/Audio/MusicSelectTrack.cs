using UnityEngine;

public class MusicSelectTrack : MonoBehaviour
{
    public MusicCue musicCue;
    public MusicChannel musicChannel;
    void Start()
    {
        musicChannel.Raise(musicCue);
    }

}
