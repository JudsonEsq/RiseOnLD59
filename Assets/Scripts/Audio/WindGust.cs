using UnityEngine;
using FMODUnity;
using FMOD.Studio;


    [System.Serializable]
    public struct WindGustDurationRange
    {
        public float min;
        public float max;
    }

        [System.Serializable]
    public struct WindGustSpeedRange
    {
        public float min;
        public float max;
    }


public class WindGust : MonoBehaviour
{

    public WindGustDurationRange durationRange;
    public WindGustSpeedRange speedRange;

    private float windSpeed = 5f;

    private Vector2 randomDirection;
    private float duration = 0.5f;

    public EventReference windSound;
    private EventInstance windInstance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        duration = UnityEngine.Random.Range(durationRange.min, durationRange.max);
        windSpeed = UnityEngine.Random.Range(speedRange.min, speedRange.max);
        
        randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

        //create a sound instance
        windInstance = RuntimeManager.CreateInstance(windSound);
        windInstance.start();

        RuntimeManager.AttachInstanceToGameObject(windInstance,gameObject);


        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(randomDirection * windSpeed * Time.deltaTime, Space.World);
    }

    void OnDestroy()
    {
        windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        windInstance.release();
    }
}
