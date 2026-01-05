using System;
using System.Collections.Generic;
using System.Linq;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.Spawner;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem.QuestSteps
{
    public class KillEnemiesQuestStep : QuestStep
    {
        [SerializeField] private string enemyType;
        [SerializeField] private int killCountNeeded = 1;

        [field: SerializeField] public override string StepDescription { get; protected set; }
        [field: SerializeField] public override int GoldReward { get; protected set; } = 200;

        public override event Action<int, int> ProgressChanged;

        private readonly List<Spawner2D> enemySpawners = new();
        private readonly List<HealthModule> subscribedNonSpawnerEnemies = new();

        private int currentKillCount;
        private bool hasRegisteredSpawnerEvents;

        private void Start()
        {
            CacheRelevantSpawners();
            RegisterSpawnerEvents();
            RegisterNonSpawnerEnemies();

            ProgressChanged?.Invoke(currentKillCount, killCountNeeded);
        }

        private void OnEnable()
        {
            RegisterSpawnerEvents();
        }

        private void OnDisable()
        {
            UnregisterSpawnerEvents();
        }

        private void OnDestroy()
        {
            CleanupAllSubscriptions();
        }

        private void CacheRelevantSpawners()
        {
            enemySpawners.Clear();

            var spawners = FindObjectsByType<Spawner2D>(FindObjectsSortMode.None);
            foreach (var spawner in spawners)
            {
                if (spawner.SpawnPrefabs.Any(p => p.EnemyType == enemyType))
                {
                    enemySpawners.Add(spawner);
                }
            }
        }

        private void RegisterNonSpawnerEnemies()
        {
            var allEnemies = FindObjectsByType<Agent>(FindObjectsSortMode.None);

            foreach (var agent in allEnemies.Where(a => a.EnemyType == enemyType))
            {
                var health = agent.GetComponent<HealthModule>();
                if (health == null)
                    continue;

                health.Died += OnNonSpawnerEnemyDied;
                subscribedNonSpawnerEnemies.Add(health);
            }
        }

        private void RegisterSpawnerEvents()
        {
            if (hasRegisteredSpawnerEvents || enemySpawners.Count == 0)
                return;

            foreach (var spawner in enemySpawners)
            {
                spawner.EnemyDied += OnSpawnerEnemyDied;
            }

            hasRegisteredSpawnerEvents = true;
        }

        private void UnregisterSpawnerEvents()
        {
            if (!hasRegisteredSpawnerEvents)
                return;

            foreach (var spawner in enemySpawners)
            {
                spawner.EnemyDied -= OnSpawnerEnemyDied;
            }

            hasRegisteredSpawnerEvents = false;
        }

        private void OnSpawnerEnemyDied(Agent agent)
        {
            if (agent.EnemyType != enemyType)
                return;

            HandleEnemyKilled();
        }

        private void OnNonSpawnerEnemyDied(HealthModule module)
        {
            module.Died -= OnNonSpawnerEnemyDied;
            subscribedNonSpawnerEnemies.Remove(module);

            HandleEnemyKilled();
        }

        private void HandleEnemyKilled()
        {
            currentKillCount++;
            ProgressChanged?.Invoke(currentKillCount, killCountNeeded);

            if (currentKillCount >= killCountNeeded)
            {
                CleanupAllSubscriptions();
                FinishQuestStep();
            }
        }

        private void CleanupAllSubscriptions()
        {
            UnregisterSpawnerEvents();

            foreach (var health in subscribedNonSpawnerEnemies)
            {
                if (health != null)
                    health.Died -= OnNonSpawnerEnemyDied;
            }

            subscribedNonSpawnerEnemies.Clear();
        }

        public override (int, int) GetProgress()
        {
            return isFinished
                ? (1, 1)
                : (currentKillCount, killCountNeeded);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (killCountNeeded <= 0)
            {
                killCountNeeded = 1;
                Debug.LogWarning("[KillEnemiesQuestStep] Kill count needed must be at least 1.");
            }
        }
#endif
    }
}
