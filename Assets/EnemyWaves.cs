using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyWaves : MonoBehaviour
{
    [SerializeField] private TankHealth tank;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Collider floor;
    [SerializeField] private GameObject enemyPrefab;
    public int WaveNumber { get; private set; }
    public int EnemiesAlive => enemies.Count;
    public int EnemiesDestroyed { get; private set; }
    public void EnemyDefeated()
    {
        EnemiesDestroyed++;
        if (EnemiesDestroyed % 5 == 0) SpeedPickup.Spawn(floor);
    }
    private readonly List<EnemyAttack> enemies = new List<EnemyAttack>();

    private void Start()
    {
        if (tank == null || floor == null || enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemyWaves needs the enemy prefab, tank, floor, and spawn points assigned.");
            enabled = false;
            return;
        }
        SpawnWave();
    }

    private void Update()
    {
        enemies.RemoveAll(enemy => enemy == null);
        if (tank != null && tank.IsAlive && enemies.Count == 0) SpawnWave();
    }

    private void SpawnWave()
    {
        WaveNumber++;
        int count = WaveNumber + 3;
        for (int i = 0; i < count; i++)
        {
            Transform spawn = spawnPoints[i % spawnPoints.Length];
            Vector3 position = spawn.position;
            int ring = i / spawnPoints.Length;
            position += new Vector3(ring * 1.2f, 0f, 0f);
            position.y = floor.bounds.max.y;
            GameObject enemy = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
            enemy.name = "Enemy Wave " + WaveNumber + " - " + (i + 1);
            // Respect the prefab's pivot and size when placing it on the floor.
            Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);
                enemy.transform.position += Vector3.up * (floor.bounds.max.y - bounds.min.y);
            }
            EnemyAttack attack = enemy.GetComponent<EnemyAttack>();
            if (attack == null) attack = enemy.AddComponent<EnemyAttack>();
            attack.Initialize(tank);
            enemies.Add(attack);
        }
    }

}
