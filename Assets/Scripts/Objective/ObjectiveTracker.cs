using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    [SerializeField]
    private int NumAntsToComplete = 10;

    [SerializeField]
    private int ObjectiveNumber = 0;

    [SerializeField]
    private string ObjectiveName = "Name";

    [SerializeField]
    private string ObjectiveDescription = "Description";

    [SerializeField]
    private GameObject ObjectivePanel;
    private ObjectiveController objectiveController;

    private int NumAntsSoFar = 0;
    private bool isComplete = false;

    public void Start()
    {
        objectiveController = ObjectivePanel.GetComponent<ObjectiveController>();
        objectiveController.AddObjective(ObjectiveNumber, ObjectiveName, ObjectiveDescription);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Ant")
        {
            NumAntsSoFar++;
            Debug.Log("Objective has made progress: " + NumAntsSoFar);
            if (NumAntsSoFar >= NumAntsToComplete && !isComplete)
            {
                isComplete = true;
                FinishObjective(ObjectiveNumber);
            }
        }
    }

    void FinishObjective(int number)
    {
        Debug.Log("Finished objective: " + number);
        objectiveController.CompleteObjective(number);
        Destroy(this.gameObject);
    }
}
