using EchoesOfEtherion.QuestSystem;
using EchoesOfEtherion.QuestSystem.QuestSteps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.QuestSystem.UI
{
    public class QuestTrackerUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField]
        private GameObject questTrackerPanel;
        [SerializeField]
        private TextMeshProUGUI questTitleText;
        [SerializeField]
        private TextMeshProUGUI questDescriptionText;
        [SerializeField]
        private TextMeshProUGUI questStepText;
        [SerializeField]
        private TextMeshProUGUI stepDescriptionText;
        [SerializeField]
        private TextMeshProUGUI questProgreessPercentageText;
        [SerializeField]
        private Slider questProgressSlider;
        private QuestInfoSO trackedQuest;
        private QuestStep currentQuestStep;

        public void StartTrackingQuest(QuestInfoSO questInfo, QuestStep questStep, uint stepIndex)
        {
            trackedQuest = questInfo;
            currentQuestStep = questStep;

            questTrackerPanel.SetActive(true);

            string title = trackedQuest.DisplayName;
            string description = trackedQuest.Description;
            string questStepUI = $"Step: {stepIndex + 1}/{trackedQuest.QuestStepPrefabs.Length}";
            string stepDescription = currentQuestStep.StepDescription;

            questTitleText.text = title;
            questDescriptionText.text = description;
            questStepText.text = questStepUI;
            stepDescriptionText.text = stepDescription;

            (int, int) progress = currentQuestStep.GetProgress();
            UpdateProgressPercentage(progress.Item1, progress.Item2);

            currentQuestStep.ProgressChanged += UpdateProgressPercentage;
        }

        private void UpdateProgressPercentage(int currentProgress, int totalProgress)
        {
            string progressPercentage = $"{currentProgress}/{totalProgress}";

            questProgreessPercentageText.text = progressPercentage;
            questProgressSlider.maxValue = totalProgress;
            questProgressSlider.value = currentProgress;

            if (currentProgress >= totalProgress)
            {
                stepDescriptionText.text = "<i>Completed.</i>";
            }
        }

        public void StopTrackingQuest()
        {
            trackedQuest = null;

            if (currentQuestStep != null)
                currentQuestStep.ProgressChanged -= UpdateProgressPercentage;

            currentQuestStep = null;
            questTrackerPanel.SetActive(false);
        }
    }
}