using UnityEngine;
using FMODUnity;
using FMOD.Studio;


[System.Serializable]
public struct WindGustSpawnRange
    {
        public float min;
        public float max;
    }

public class WindGustSpawner : MonoBehaviour
{    private Vector3 spawnLocation;

    private float spawnTimer;
    public WindGustSpawnRange WindGustIntervalRange;

    public WindGust windGustPrefab;

    [UnityEngine.Range(0f,1f)]
    public float offScreenSpawnChance = 0.5f;

    public EventReference bgAmbienceLp;

    public float ySpawnHeight = 2f;
    // Update is called once per frame

    private EventInstance bgAmbienceInstance;
    
    void Start()
    {
        bgAmbienceInstance = RuntimeManager.CreateInstance(bgAmbienceLp);
        bgAmbienceInstance.start();

    }
    
    
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0f)
        {
            SpawnGustOfWind();
            spawnTimer = Random.Range(WindGustIntervalRange.min, WindGustIntervalRange.max);
        }
    }

    private void SpawnGustOfWind()
    {
        if(Random.value < offScreenSpawnChance)
            spawnLocation = GetRandomScreenPosition(true);
        else
            spawnLocation = GetRandomScreenPosition(false);

        Instantiate(windGustPrefab, spawnLocation,Quaternion.identity);
    }

    private Vector3 GetRandomScreenPosition(bool isOffScreen)
    {
        float x = 0f;
        float z = 0f;

        if(!isOffScreen)
        {
            x = Random.Range(0f,1f);
            z = Random.Range(0f,1f);
        }
        else
        {
            int randomEdge = Random.Range(0,4);
            switch (randomEdge)
            {
                case 0:
                    x = -0.1f;
                    z = Random.Range(0f,1f);
                    break;
                case 1:
                    x = 1.1f;
                    z = Random.Range(0f,1f);
                    break;
                case 2:
                    x = Random.Range(0f,1f);
                    z = -0.1f;
                    break;
                case 3:
                    x = Random.Range(0f,1f);
                    z = 1.1f;
                    break;
                default:
                    break;
            }
        }
        return Camera.main.ViewportToWorldPoint(new Vector3(x, ySpawnHeight, z));
    }

    void OnDestroy()
    {
        bgAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bgAmbienceInstance.release();
    }

}
