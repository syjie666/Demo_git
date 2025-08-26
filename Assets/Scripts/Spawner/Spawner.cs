
using System;

using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("����Ԥ�����б�˳�������׵��ѣ�")]
    public List<GameObject> enemyPrefabs;

    [Header("ˢ������")]
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int maxEnemyCount = 10;

    [Header("�ݽ�����")]
    public int currentMaxEnemyIndex = 0;      // ��ǰ��ˢ����������������
    public float timeToIncreaseDifficulty = 30f; // ÿ�����뿪�Ÿ����Ѷȹ���
    public int killsToIncreaseDifficulty = 5;    // ÿɱ���ٹֿ��Ÿ����Ѷȹ���

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
            //Debug.LogWarning("enemyPrefabsΪ�ջ�currentMaxEnemyIndexԽ��");
            return;
        }

        int index = UnityEngine.Random.Range(0, currentMaxEnemyIndex + 1);
        GameObject prefab = enemyPrefabs[index];
        //Debug.Log($"���ɹ���: {prefab.name} ����: {index}");

        if (spawnPoints.Length == 0)
        {
            //Debug.LogWarning("spawnPointsΪ��");
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
            //Debug.Log($"�Ѷ����������Ź�������������{currentMaxEnemyIndex}");
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
