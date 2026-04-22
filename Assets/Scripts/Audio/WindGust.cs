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

    private Vector3 randomDirection;
    private float duration = 0.5f;

    public EventReference windSound;
    private EventInstance windInstance;

    void Start()
    {
        duration = Random.Range(durationRange.min, durationRange.max);
        windSpeed = Random.Range(speedRange.min, speedRange.max);
        
        Vector2 circle = Random.insideUnitCircle.normalized;
        randomDirection = new Vector3(circle.x, 0f, circle.y);

        //create a sound instance
        windInstance = RuntimeManager.CreateInstance(windSound);
        windInstance.start();

        //used to attach audio to the spawned invisible game object and update it's position
        RuntimeManager.AttachInstanceToGameObject(windInstance,gameObject);


        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.Translate(randomDirection * windSpeed * Time.deltaTime, Space.World);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.azure;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void OnDestroy()
    {
        windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        windInstance.release();
    }
}
