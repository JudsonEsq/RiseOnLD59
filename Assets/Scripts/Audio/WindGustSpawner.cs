using UnityEngine;
using FMODUnity;


[System.Serializable]
public struct WindGustSpawnRange
    {
        public float min;
        public float max;
    }

public class WindGustSpawner : MonoBehaviour
{    private Vector2 spawnLocation;

    private float spawnTimer;
    public WindGustSpawnRange WindGustIntervalRange;

    public WindGust windGustPrefab;

    [UnityEngine.Range(0f,1f)]
    public float offScreenSpawnChance = 0.5f;

    public EventReference bgAmbienceLp;
    // Update is called once per frame
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

    private Vector2 GetRandomScreenPosition(bool isOffScreen)
    {
        float x = 0f;
        float y = 0f;

        if(!isOffScreen)
        {
            x = Random.Range(0f,1f);
            y = Random.Range(0f,1f);
        }
        else
        {
            int randomEdge = Random.Range(0,4);
            switch (randomEdge)
            {
                case 0:
                    x = -0.1f;
                    y = Random.Range(0f,1f);
                    break;
                case 1:
                    x = 1.1f;
                    y = Random.Range(0f,1f);
                    break;
                case 2:
                    x = Random.Range(0f,1f);
                    y = -0.1f;
                    break;
                case 3:
                    x = Random.Range(0f,1f);
                    y = 1.1f;
                    break;
                default:
                    break;
            }
        }
        return Camera.main.ViewportToWorldPoint(new Vector3(x, y, 0f));
    }
}
