using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;

    public float spawnInterval = 5f;

    void Start()
    {
        Scheduler.instance.Schedule(spawnInterval, true, Spawn);
    }

    void Update()
    {
        
    }

    void Spawn()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
