using UnityEngine;
using UnityEngine.InputSystem;

public class TestGameManager : MonoBehaviour
{
    public GameObject mainNest;
    public GameObject antPrefab;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.nKey.isPressed)
        {
            Instantiate(antPrefab, mainNest.transform.position, Quaternion.identity).GetComponent<AntController>().nest = mainNest;
        }

        if (Keyboard.current.fKey.isPressed)
        {
            Time.timeScale = 2f;
        }
    }
}
