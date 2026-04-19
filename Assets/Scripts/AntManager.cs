using NUnit.Framework;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AntManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Ants = new List<GameObject>();


    [SerializeField]
    private GameObject AntPrefab;

    [SerializeField]
    private int AntDeadTimeout = 5;

    [SerializeField]
    private TMP_Text CostText;

    [SerializeField]
    private TMP_Text AntsText;

    private void Update()
    {
        // remove any ants that are dead
        List<GameObject> deadAnts = new List<GameObject>();   
        foreach (GameObject ant in Ants)
        {
            AntController antCtrl = ant.GetComponent<AntController>();
            if (antCtrl.IsDead())
            {
                IEnumerator coroutine = RemoveAnt(ant);
                StartCoroutine(coroutine);
                deadAnts.Add(ant);
            }
        }

        foreach (GameObject ant in deadAnts)
        {
            ant.transform.rotation *= Quaternion.Euler(180, 0, 0);
            Ants.Remove(ant);
        }

        CostText.text = GetAntFoodCost().ToString();
        AntsText.text = Ants.Count.ToString();
    }

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
            if (!antCtrl.IsDead()) 
            { 
                totalFood += antCtrl.FoodCost();
            }
        }
        return totalFood;
    }

    public void CullAnts(int debitFood)
    {
        while (debitFood > 0)
        {
            Debug.Log("Not Enough Food, culling ants: " + debitFood);
            int antIdx = Random.Range(0, Ants.Count);
            AntController ant = Ants[antIdx].GetComponent<AntController>();
            debitFood -= ant.FoodCost();
            ant.Kill();
        }

    }

    IEnumerator RemoveAnt(GameObject ant)
    {
        yield return new WaitForSeconds(AntDeadTimeout);
        Destroy(ant);
    }
}
