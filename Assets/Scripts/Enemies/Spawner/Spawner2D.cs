using System;
using System.Collections.Generic;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EchoesOfEtherion.Enemies.Spawner
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Spawner2D : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [field: SerializeField] public Agent[] SpawnPrefabs { get; private set; }
        [field: SerializeField] public int MaxCount { get; private set; } = 5;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask enemiesLayer;
        [SerializeField] private float spawnCheckRadius = 32f;


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

            // Keep trying to get a valid point
            Vector2 spawnPos = GetValidSpawnPosition();

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
        
        private Vector2 GetValidSpawnPosition()
        {
            int attempts = 0;

            while (attempts < 50)
            {
                attempts++;

                Vector2 point = GetRandomPointInPolygon();

                bool blocked = Physics2D.OverlapCircle(point, spawnCheckRadius, playerLayer | enemiesLayer);

                if (!blocked)
                    return point;
            }

            return GetRandomPointInPolygon();
        }

        private Vector2 GetRandomPointInPolygon()
        {
            Bounds bounds = polygon.bounds;
            Vector2 point;

            for (int i = 0; i < 50; i++)
            {
                point = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                if (polygon.OverlapPoint(point))
                    return point;
            }

            return polygon.ClosestPoint(transform.position);
        }

    }
}
