using System;
using System.Collections.Generic;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EchoesOfEtherion.Spawner
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Spawner2D : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject[] spawnPrefabs;
        public int maxCount = 5;

        [Tooltip("Base spawn time in seconds.")]
        public float spawnInterval = 5f;

        [Tooltip("Random extra seconds added/subtracted from the base interval.")]
        public float spawnIntervalRandom = 2f;

        [Tooltip("Multiplier that increases interval based on density (0 = off)")]
        public float densityMultiplier = 3f;

        private float timer;
        private readonly List<GameObject> spawnedObjects = new List<GameObject>();
        private PolygonCollider2D polygon;

        public event Action<GameObject> OnSpawned;
        public event Action<GameObject> OnObjectDied;

        private void Awake()
        {
            polygon = GetComponent<PolygonCollider2D>();
        }

        private void Update()
        {
            CleanSpawnList();

            if (spawnedObjects.Count >= maxCount)
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
            float density = (float)spawnedObjects.Count / maxCount;
            float densityScale = 1f + density * densityMultiplier;

            timer = (spawnInterval * densityScale) + Random.Range(-spawnIntervalRandom, spawnIntervalRandom);
            timer = Mathf.Max(0.1f, timer);
        }

        private void SpawnRandomObject()
        {
            if (spawnPrefabs.Length == 0) return;

            GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            Vector2 spawnPos = GetRandomPointInPolygon();

            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
            spawnedObjects.Add(instance);

            if (instance.TryGetComponent(out HealthModule health))
            {
                health.Died += OnDied;
            }

            OnSpawned?.Invoke(instance);
        }

        private void OnDied(HealthModule module)
        {
            OnObjectDied?.Invoke(module.gameObject);
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
