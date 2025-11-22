using System;
using System.Collections.Generic;
using System.Linq;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Game.Locations;
using EchoesOfEtherion.Enemies.Spawner;
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

        private List<Spawner2D> thisEnemySpawner = new();

        private int currentKillCount = 0;
        private bool hasRegisteredEnemyDied = false;

        private void Start()
        {
            var spawners = FindObjectsByType<Spawner2D>(FindObjectsSortMode.None);

            thisEnemySpawner = new();

            foreach (Spawner2D spawner in spawners)
            {
                if (spawner.SpawnPrefabs.Any(a => a.EnemyType == enemyType))
                {
                    thisEnemySpawner.Add(spawner);
                }
            }

            ProgressChanged?.Invoke(currentKillCount, killCountNeeded);

            RegisterEnemyDied();
        }

        private void OnEnable()
        {
            RegisterEnemyDied();
        }

        private void OnDisable()
        {
            UnRegisterEnemyDied();
        }

        private void RegisterEnemyDied()
        {
            if (thisEnemySpawner.Count > 0 && !hasRegisteredEnemyDied)
            {
                foreach (Spawner2D spawner in thisEnemySpawner)
                {
                    spawner.EnemyDied += OnEnemyDied;
                }

                hasRegisteredEnemyDied = true;
            }
        }

        private void UnRegisterEnemyDied()
        {
            if (thisEnemySpawner.Count > 0 && hasRegisteredEnemyDied)
            {
                foreach (Spawner2D spawner in thisEnemySpawner)
                {
                    spawner.EnemyDied -= OnEnemyDied;
                }

                hasRegisteredEnemyDied = false;
            }
        }

        private void OnEnemyDied(Agent agent)
        {
            if (agent.EnemyType == enemyType)
            {
                currentKillCount++;

                ProgressChanged?.Invoke(currentKillCount, killCountNeeded);

                if (currentKillCount >= killCountNeeded)
                {
                    UnRegisterEnemyDied();
                    FinishQuestStep();
                }
            }
        }

        public override (int, int) GetProgress()
        {
            return isFinished ? (1, 1) : (currentKillCount, killCountNeeded);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (killCountNeeded <= 0)
            {
                killCountNeeded = 1;
                Debug.LogWarning($"[KillEnemiesQuestStep] Kill count needed must be at least 1!");
            }
        }
#endif
    }
}
