using EchoesOfEtherion.QuestSystem.QuestSteps;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem
{
    public class Quest
    {
        public QuestInfoSO QuestInfo { get; private set; }

        public QuestState state;

        public uint CurrentQuestStepIndex { get; private set; }

        public Quest(QuestInfoSO questInfo)
        {
            QuestInfo = questInfo;
            state = QuestState.RequirementsNotMet;
            CurrentQuestStepIndex = 0;
        }

        public void MoveToNextStep()
        {
            CurrentQuestStepIndex++;
        }

        public bool CurrentStepExists()
        {
            return CurrentQuestStepIndex < QuestInfo.QuestStepPrefabs.Length;

        }
        public void InstantiateCurrentQuestStepPrefab(Transform parent)
        {
            GameObject questStepPrefab = GetCurrentQuestStepPrefab();
            if (questStepPrefab != null)
            {
                QuestStep questStep = GameObject.Instantiate(questStepPrefab, parent)
                    .GetComponent<QuestStep>();

                questStep.InitializeQuestStep(QuestInfo.ID);

            }
        }
        public GameObject GetCurrentQuestStepPrefab()
        {
            GameObject questStepPrefab = null;
            if (CurrentStepExists())
            {
                questStepPrefab = QuestInfo.QuestStepPrefabs[CurrentQuestStepIndex];
            }
            else
                Debug.LogWarning($"No quest step exists at index {CurrentQuestStepIndex} for quest {QuestInfo.DisplayName}");

            return questStepPrefab;
        }
    }
}