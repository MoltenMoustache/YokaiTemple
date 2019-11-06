using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiSpawner : MonoBehaviour
{
    [SerializeField] GameObject yokaiPrefab;
    [SerializeField] GameObject[] spawnPositions;
    [SerializeField] int numberOfYokai;
    [SerializeField] float spawnDelay;
    [SerializeField] float yureiChance;
    [SerializeField] float laternChance;
    float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        if(currentTime <= 0)
        {
            int spawnNumber = Random.Range(1, numberOfYokai);
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(yokaiPrefab, spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position, transform.rotation);
            }
            currentTime = spawnDelay;
        }
    }
}
