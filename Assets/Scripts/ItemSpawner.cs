﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct Spawnable
    {
        public GameObject gameObject;

        public float weight;
    }

    public List<Spawnable> items = new List<Spawnable>();

    float totalWeight;

    void Awake()
    {
        totalWeight = 0;
        foreach(var spawnable in items)
        {
            totalWeight += spawnable.weight;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        float pick = Random.value * totalWeight;
        int chosenIndex = 0;
        float cumulativeWeight = items[0].weight;

        while (pick > cumulativeWeight && chosenIndex < items.Count - 1)
        {
            chosenIndex++;
            cumulativeWeight += items[chosenIndex].weight;
        }
        //啥意思
        GameObject i = Instantiate(items[chosenIndex].gameObject, transform.position, Quaternion.identity) as GameObject;
    }

    public void dropItemAftherEnemyDeath(Vector3 position)
    {
        int itemIndex = Random.Range(0, items.Count);
        if(itemIndex < items.Count) {
            GameObject i = Instantiate(items[itemIndex].gameObject, position, Quaternion.identity) as GameObject;
        }

    }
   
}

