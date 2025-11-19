using System;
using System.Collections.Generic;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EchoesOfEtherion.Spawner
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Spawner2D : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [field: SerializeField] public Agent[] SpawnPrefabs { get; private set; }
        [field: SerializeField] public int MaxCount { get; private set; } = 5;

        [Tooltip("Base spawn time in seconds.")]
        [SerializeField] private float spawnInterval = 5f;

        [Tooltip("Random extra seconds added/subtracted from the base interval.")]
        [SerializeField] private float spawnIntervalRandom = 2f;

        [Tooltip("Multiplier that increases interval based on density (0 = off)")]
        [SerializeField] private float densityMultiplier = 1.25f;

        private float timer;
        private readonly List<Agent> spawnedObjects = new();
        private PolygonCollider2D polygon;

        public event Action<Agent> EnemySpawned;
        public event Action<Agent> EnemyDied;

        private void Awake()
        {
            polygon = GetComponent<PolygonCollider2D>();
        }

        private void Update()
        {
            CleanSpawnList();

            if (spawnedObjects.Count >= MaxCount)
                return;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SpawnRandomObject();
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            float density = (float)spawnedObjects.Count / MaxCount;
            float densityScale = 1f + density * densityMultiplier;

            timer = (spawnInterval * densityScale) + Random.Range(-spawnIntervalRandom, spawnIntervalRandom);
            timer = Mathf.Max(0.1f, timer);
        }

        private void SpawnRandomObject()
        {
            if (SpawnPrefabs.Length == 0) return;

            Agent prefab = SpawnPrefabs[Random.Range(0, SpawnPrefabs.Length)];
            Vector2 spawnPos = GetRandomPointInPolygon();

            Agent instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
            spawnedObjects.Add(instance);

            if (instance.TryGetComponent(out HealthModule health))
            {
                health.Died += OnDied;
            }

            EnemySpawned?.Invoke(instance);
        }

        private void OnDied(HealthModule module)
        {
            EnemyDied?.Invoke(module.GetComponent<Agent>());
        }

        private void CleanSpawnList()
        {
            spawnedObjects.RemoveAll(item => item == null);
        }

        private Vector2 GetRandomPointInPolygon()
        {
            Bounds bounds = polygon.bounds;
            Vector2 point;
            int safety = 0;

            do
            {
                point = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                safety++;

                if (safety > 40)
                    return polygon.ClosestPoint(point);

            } while (!polygon.OverlapPoint(point));

            return point;
        }
    }
}
