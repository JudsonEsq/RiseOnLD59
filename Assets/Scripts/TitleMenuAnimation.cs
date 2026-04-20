using System;
using Unity.VisualScripting;
using UnityEngine;

public class TitleMenuAnimation : MonoBehaviour
{
    public GameObject[] titleLetters;
    private float[] originalPos;
    public float timer;
    public int interval = 0;

    [SerializeField, Tooltip("How high the letters go up")] private float amplitude = 1f;
    [SerializeField, Tooltip("How often the letters move")] private float frequency = 1f;
    [SerializeField, Tooltip("Lowest value the position should clamp")] private float floor = 0.3f;
    [SerializeField, Tooltip("Highest value the position should clamp")] private float ceil = 3f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0f;

        originalPos = new float[titleLetters.Length];
        for(int i = 0; i < titleLetters.Length; i++)
        {
            originalPos[i] = titleLetters[i].transform.position.y;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        timer += 10 * Time.deltaTime;
        if(timer >= interval)
        {
            for(int i = 0; i < titleLetters.Length; i++)
            {               
                Transform tempLetter = titleLetters[i].transform;
                Vector3 oldPos = tempLetter.position;
                oldPos.y = originalPos[i] + Mathf.Clamp(MathF.Sin((timer - i) * frequency) * amplitude, floor, ceil);
                tempLetter.position = oldPos;
            }
        }
    }
}
