using Assets.Scripts.Objective;
using Unity.VisualScripting;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    // How far are we in the game's progression?
    private int objectivePhase = 0;
    // How many total phases does the game have?
    private int gameLength = 5;

    [SerializeField]
    private ObjectiveController controller;

    bool boardSet = false;

    private void Start()
    {
        setBoard();
    }

    private void Update()
    {
        switch (objectivePhase)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    private void setBoard()
    {
        switch (objectivePhase)
        {
            case 0:
                
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

}
