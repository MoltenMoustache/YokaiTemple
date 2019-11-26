using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyObject
{
    public GameObject enemyType;
    public int spawnChance;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies")]
    public EnemyObject[] enemyTypes;
    [SerializeField] float spawnMinimumCooldown;
    [SerializeField] float spawnMaxCooldown;
    [SerializeField] GameObject[] spawnPositions;

    // Start is called before the first frame update
    void Start()
    {
        int maxSpawnChance = 0;

        foreach (var type in enemyTypes)
        {
            maxSpawnChance += type.spawnChance;
        }

        StartCoroutine(SelectEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SelectEnemy()
    {
        float cooldown = Random.Range(spawnMinimumCooldown, spawnMaxCooldown);

        yield return new WaitForSeconds(cooldown);

        int spawnChance = Random.Range(1, 100);
        if (spawnChance < enemyTypes[0].spawnChance)
        {
            SpawnEnemy(enemyTypes[0].enemyType);
        }
        else if (spawnChance > enemyTypes[0].spawnChance && spawnChance < enemyTypes[0].spawnChance + enemyTypes[1].spawnChance)
        {
            SpawnEnemy(enemyTypes[1].enemyType);
        }
        else
        {
            SpawnEnemy(enemyTypes[2].enemyType);
        }

        StartCoroutine(SelectEnemy());
    }

    void SpawnEnemy(GameObject a_enemyType)
    {
        // Select spawn position
        GameObject spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];

        Instantiate(a_enemyType, spawnPos.transform.position, Quaternion.identity);
    }
}
