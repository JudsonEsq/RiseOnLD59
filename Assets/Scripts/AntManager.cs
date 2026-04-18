using NUnit.Framework;
using UnityEngine;

using System.Collections.Generic;

public class AntManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Ants = new List<GameObject>();


    [SerializeField]
    private GameObject AntPrefab;

    [SerializeField]
    private int AntDeadTimeout = 5;


    public GameObject SpawnAnt(Transform location)
    {
        GameObject newAnt = Instantiate(AntPrefab, location);
        newAnt.GetComponent<AntController>().nest = this.gameObject;
        Ants.Add(newAnt);

        return newAnt;
    }

    public int GetAntFoodCost()
    {
        int totalFood = 0;
        foreach (GameObject ant in Ants)
        {
            AntController antCtrl = ant.GetComponent<AntController>();
            totalFood += antCtrl.FoodCost();
        }
        return totalFood;
    }

    public void CullAnts(int debitFood)
    {
        while (debitFood > 0)
        {
            int antIdx = Random.Range(0, Ants.Count);
            AntController ant = Ants[antIdx].GetComponent<AntController>();
            debitFood -= ant.FoodCost();
            ant.Kill();
        }

        // remove any ants that were made dead
        List<GameObject> deadAnts = new List<GameObject>();   
        foreach (GameObject ant in Ants)
        {
            AntController antCtrl = ant.GetComponent<AntController>();
            if (antCtrl.IsDead())
            {
                Invoke("DestroyAnt", AntDeadTimeout);
                deadAnts.Add(ant);
            }
        }

        foreach (GameObject ant in deadAnts)
        {
            Ants.Remove(ant);
        }
    }

    void RemoveAnt(GameObject ant)
    {
        Destroy(ant);
    }
}
