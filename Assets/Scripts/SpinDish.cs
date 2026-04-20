using UnityEngine;

public class SpinDish : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 200 * Time.deltaTime, 0));
    }
}
