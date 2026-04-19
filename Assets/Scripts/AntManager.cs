using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

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

    [SerializeField]
    private TMP_Text StarveText;
    [SerializeField]
    private int StarveTextTimeout = 5;

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

    private void DisableStarveText()
    {
        StartCoroutine(FadeTextToZeroAlpha(1f, StarveText));
        //StarveText.gameObject.SetActive(false);
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
        int CountDeadAnts = 0;
        while (debitFood > 0)
        {
            Debug.Log("Not Enough Food, culling ants: " + debitFood);
            int antIdx = UnityEngine.Random.Range(0, Ants.Count);
            AntController ant = Ants[antIdx].GetComponent<AntController>();
            debitFood -= ant.FoodCost();
            ant.Kill();
            CountDeadAnts++;
        }

        if (CountDeadAnts > 0)
        {
            StarveText.text = CountDeadAnts + " of your Ants starved!";
            StartCoroutine(FadeTextToFullAlpha(1f, StarveText));
            //StarveText.gameObject.SetActive(true);

            Invoke("DisableStarveText", StarveTextTimeout);
        }

    }

    IEnumerator RemoveAnt(GameObject ant)
    {
        yield return new WaitForSeconds(AntDeadTimeout);
        Destroy(ant);
    }

    public IEnumerator FadeTextToFullAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
