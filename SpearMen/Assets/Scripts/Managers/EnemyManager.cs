using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints = new List<Transform>();

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int maxEnemies = 0;

    private void Start()
    {
        GameObject[] startingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        

        foreach (GameObject enemy in startingEnemies)
        {
            activeEnemies.Add(enemy);
        }

        maxEnemies = activeEnemies.Count;
    }

    private void Update()
    {
        CleanUpDeadEnemies();

        while (activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Count == 0)
            return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        newEnemy.tag = "Enemy";
        activeEnemies.Add(newEnemy);
    }

    private void CleanUpDeadEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                activeEnemies.RemoveAt(i);
            }
        }
    }
}
