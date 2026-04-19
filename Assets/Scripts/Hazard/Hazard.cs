using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hazard Collision");
        if (other.GetComponent<Collider>().tag == "Ant")
        {
            Debug.Log("Ant on the Hazard!");
            AntController ant = other.gameObject.GetComponent<AntController>();
            ant.Kill();
        }
    }
}
