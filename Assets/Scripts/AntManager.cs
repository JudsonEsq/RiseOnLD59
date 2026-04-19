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
    private GameObject WorkerPrefab;
    [SerializeField]
    private GameObject SoldierPrefab;
    [SerializeField]
    private GameObject FirePrefab;
    [SerializeField]
    private GameObject CarpenterPrefab;

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

    private GameObject GetPrefabForType(AntController.AntType type)
    {
        switch (type)
        {
            case AntController.AntType.Worker:
                return WorkerPrefab;
            case AntController.AntType.Soldier:
                return SoldierPrefab;
            case AntController.AntType.Fire:
                return FirePrefab;
            case AntController.AntType.Carpenter:
                return CarpenterPrefab;
            default:
                break;
        }
        return WorkerPrefab;
    }

    public GameObject SpawnAnt(Transform location, AntController.AntType type)
    {
        GameObject prefab = GetPrefabForType(type);
        GameObject newAnt = Instantiate(prefab, location);
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
