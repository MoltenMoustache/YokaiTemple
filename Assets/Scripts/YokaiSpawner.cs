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

    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        currentTime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.CheckAllPlayersDowned())
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
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
}
