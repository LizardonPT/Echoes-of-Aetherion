using System.Collections.Generic;
using EchoesOfEtherion.QuestSystem.QuestSteps;
using EchoesOfEtherion.QuestSystem.UI;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem
{
    [RequireComponent(typeof(QuestTrackerUI))]
    public class QuestManager : Singleton<QuestManager>
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;
        public QuestEvents QuestEvents { get; private set; } = new();
        private Dictionary<int, Quest> questMap = new();

        //todo: Real level up system. For now, just a placeholder.
        private int playerLevel = 500;
        private QuestTrackerUI questTrackerUI;

        protected override void Initialize()
        {
            questMap = CreateQuestMap();
            questTrackerUI = GetComponent<QuestTrackerUI>();
        }

        private void OnEnable()
        {
            QuestEvents.StartQuest += StartQuest;
            QuestEvents.FinishQuest += FinishQuest;
            QuestEvents.AdvanceQuestStep += AdvanceQuestStep;
        }

        private void OnDisable()
        {
            QuestEvents.StartQuest -= StartQuest;
            QuestEvents.FinishQuest -= FinishQuest;
            QuestEvents.AdvanceQuestStep -= AdvanceQuestStep;
        }

        private void Start()
        {
            foreach (Quest quest in questMap.Values)
            {
                QuestEvents.OnQuestStateChanged(quest);
            }
        }

        private void Update()
        {
            //todo: Listen to Level up event isntead of checking every frame
            foreach (Quest quest in questMap.Values)
            {
                if (quest.state == QuestState.RequirementsNotMet && CheckRequirementsMet(quest))
                {
                    ChangeQuestState(quest.QuestInfo.ID, QuestState.CanStart);
                }
            }
        }

        private bool CheckRequirementsMet(Quest quest)
        {
            bool meetsRequirements = quest.QuestInfo.RequiredLevel <= playerLevel;

            if (quest.QuestInfo.QuestPrerequisites != null && quest.QuestInfo.QuestPrerequisites.Length > 0)
            {
                foreach (QuestInfoSO prerequisiteQuest in quest.QuestInfo.QuestPrerequisites)
                {
                    if (GetQuestById(prerequisiteQuest.ID).state != QuestState.Finished)
                    {
                        meetsRequirements = false;
                        break;
                    }
                }
            }

            return meetsRequirements;
        }
        private void ChangeQuestState(int id, QuestState newState)
        {
            Quest quest = GetQuestById(id);
            if (quest != null)
            {
                quest.state = newState;
                QuestEvents.OnQuestStateChanged(quest);
            }
        }

        private void StartQuest(int id)
        {
            Log($"Starting quest with ID: {id}");
            Quest quest = GetQuestById(id);
            QuestStep questStep = quest.InstantiateCurrentQuestStepPrefab(transform);
            questTrackerUI.StartTrackingQuest(quest.QuestInfo, questStep, quest.CurrentQuestStepIndex);
            ChangeQuestState(id, QuestState.InProgress);
        }

        private void FinishQuest(int id)
        {
            Log($"Finishing quest with ID: {id}");
            Quest quest = GetQuestById(id);
            ClaimRewards(quest);
            questTrackerUI.StopTrackingQuest();
            ChangeQuestState(id, QuestState.Finished);
        }

        private void AdvanceQuestStep(int id)
        {
            Quest quest = GetQuestById(id);
            quest.MoveToNextStep();

            if (quest.CurrentStepExists())
            {
                QuestStep step = quest.InstantiateCurrentQuestStepPrefab(transform);
                questTrackerUI.StartTrackingQuest(quest.QuestInfo, step, quest.CurrentQuestStepIndex);
            }
            else
            {
                ChangeQuestState(id, QuestState.CanFinish);
            }


            Log($"Advancing quest step for quest with ID: {id}");
        }

        private Dictionary<int, Quest> CreateQuestMap()
        {
            Log("Creating quest map...");
            QuestInfoSO[] allQuestInfos = Resources.LoadAll<QuestInfoSO>("Quests");
            Log($"Found {allQuestInfos.Length} quests in Resources.");

            Dictionary<int, Quest> idToQuestMap = new();

            foreach (QuestInfoSO questInfo in allQuestInfos)
            {
                int questId = questInfo.ID;
                Log($"Processing quest: {questInfo.DisplayName} (ID: {questId})");

                if (!idToQuestMap.ContainsKey(questId))
                {
                    Quest newQuest = new(questInfo);
                    idToQuestMap.Add(questId, newQuest);
                    Log($"Added quest '{questInfo.DisplayName}' to quest map.");
                }
                else
                {
                    Debug.LogWarning($"Duplicate quest ID detected for quest '{questInfo.DisplayName}'. Skipping addition to quest map.");
                }
            }

            Log($"Quest map creation complete. Total quests: {idToQuestMap.Count}");
            return idToQuestMap;
        }

        public Quest GetQuestById(int questId)
        {
            questMap.TryGetValue(questId, out Quest quest);

            if (quest == null)
            {
                Debug.LogWarning($"Quest with ID {questId} not found.");
            }

            return quest;
        }

        private void ClaimRewards(Quest quest)
        {
            //todo: Implement gold and level up system.
        }

        private void Log(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[QuestManager] {message}");
            }
        }
    }
}