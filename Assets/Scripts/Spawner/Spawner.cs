
using System;

using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("怪物预制体列表（顺序建议由易到难）")]
    public List<GameObject> enemyPrefabs;

    [Header("刷怪设置")]
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int maxEnemyCount = 10;

    [Header("递进控制")]
    public int currentMaxEnemyIndex = 0;      // 当前可刷怪物种类上限索引
    public float timeToIncreaseDifficulty = 30f; // 每多少秒开放更高难度怪物
    public int killsToIncreaseDifficulty = 5;    // 每杀多少怪开放更高难度怪物

    private int currentEnemyCount = 0;
    private float timer = 0f;
    private float difficultyTimer = 0f;
    private int killCount = 0;

    
    public void OnEnable()
    {
        EventCenter.instance.OnEnemyKilled += HandleEnemyKilled;

    }
    public void OnDisable()
    {
        EventCenter.instance.OnEnemyKilled -= HandleEnemyKilled;

    }
    private void Update()
    {
        timer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        
        if (difficultyTimer >= timeToIncreaseDifficulty)
        {
            difficultyTimer = 0f;
            IncreaseDifficulty();
        }

        if (timer >= spawnInterval && currentEnemyCount < maxEnemyCount)
        {
            timer = 0f;
            SpawnRandomEnemy();
        }
    }


    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Count == 0 || currentMaxEnemyIndex >= enemyPrefabs.Count)
        {
            //Debug.LogWarning("enemyPrefabs为空或currentMaxEnemyIndex越界");
            return;
        }

        int index = UnityEngine.Random.Range(0, currentMaxEnemyIndex + 1);
        GameObject prefab = enemyPrefabs[index];
        //Debug.Log($"生成怪物: {prefab.name} 索引: {index}");

        if (spawnPoints.Length == 0)
        {
            //Debug.LogWarning("spawnPoints为空");
            return;
        }
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        PoolManager.Instance.Spawn(prefab.name, spawnPoint.position, Quaternion.identity);

        currentEnemyCount++;
    }


    private void IncreaseDifficulty()
    {
        if (currentMaxEnemyIndex < enemyPrefabs.Count - 1)
        {
            currentMaxEnemyIndex++;
            //Debug.Log($"难度提升，开放怪物种类索引：{currentMaxEnemyIndex}");
        }
    }

    private void HandleEnemyKilled(GameObject enemy)
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        killCount++;

        if (killCount >= killsToIncreaseDifficulty)
        {
            killCount = 0;
            IncreaseDifficulty();
        }
    }
}
