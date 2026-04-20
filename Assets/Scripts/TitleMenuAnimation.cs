using System;
using Unity.VisualScripting;
using UnityEngine;

public class TitleMenuAnimation : MonoBehaviour
{
    public GameObject[] titleLetters;
    public float timer;
    public int interval = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0f;
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
                tempLetter.position = new Vector3(tempLetter.position.x, Mathf.Clamp(3 * MathF.Sin((timer - i) / 3.5f), 0, 3), tempLetter.position.z);
            }
        }
    }
}
