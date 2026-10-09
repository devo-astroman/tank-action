using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyWaves : MonoBehaviour
{
    [SerializeField] private TankHealth tank;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Collider floor;
    public int WaveNumber { get; private set; }
    public int EnemiesAlive => enemies.Count;
    private readonly List<EnemyAttack> enemies = new List<EnemyAttack>();
    private Material enemyMaterial;

    private void Start()
    {
        if (tank == null || floor == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemyWaves needs the tank, floor, and spawn points assigned.");
            enabled = false;
            return;
        }
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        enemyMaterial = new Material(shader) { color = new Color(0.65f, 0.12f, 0.85f) };
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
            position.y = floor.bounds.max.y + 0.5f;
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "Enemy Wave " + WaveNumber + " - " + (i + 1);
            enemy.transform.position = position;
            enemy.GetComponent<Renderer>().sharedMaterial = enemyMaterial;
            EnemyAttack attack = enemy.AddComponent<EnemyAttack>();
            attack.Initialize(tank);
            enemies.Add(attack);
        }
    }

    private void OnDestroy()
    {
        if (enemyMaterial != null) Destroy(enemyMaterial);
    }
}
