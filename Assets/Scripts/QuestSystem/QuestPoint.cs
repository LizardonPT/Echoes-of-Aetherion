using UnityEngine;
using UnityEngine.PlayerLoop;

namespace EchoesOfEtherion.QuestSystem
{
    [RequireComponent(typeof(QuestIcon))]
    public class QuestPoint : MonoBehaviour
    {
        [Header("Quest")]
        [SerializeField] private QuestInfoSO questInfo;
        [Header("Settings")]
        [SerializeField] private bool startPoint = false;
        [SerializeField] private bool finishPoint = false;
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        private QuestState currentState = QuestState.RequirementsNotMet;
        private QuestIcon questIcon;

        private void Awake()
        {
            questIcon = GetComponentInChildren<QuestIcon>();
        }

        private void Start()
        {
            currentState = QuestManager.Instance.GetQuestState(questInfo.ID);
            questIcon.UpdateState(currentState, startPoint, finishPoint);
        }

        public void UpdateQuest()
        {
            Log($"IsStartPoint: {startPoint}, IsFinishPoint: {finishPoint}, CurrentState: {currentState}");
            if (startPoint && currentState == QuestState.CanStart)
            {
                QuestManager.Instance.QuestEvents.OnStartQuest(questInfo.ID);
            }
            else if (finishPoint && currentState == QuestState.CanFinish)
            {
                QuestManager.Instance.QuestEvents.OnFinishQuest(questInfo.ID);
            }
        }

        private void OnEnable()
        {
            QuestManager.Instance.QuestEvents.QuestStateChanged += OnQuestStateChanged;
        }
        private void OnDisable()
        {
            if (QuestManager.Instance == null) return;
            QuestManager.Instance.QuestEvents.QuestStateChanged -= OnQuestStateChanged;
        }

        private void OnQuestStateChanged(Quest quest)
        {
            if (quest.QuestInfo.ID != questInfo.ID)
                return;
            currentState = quest.state;
            questIcon.UpdateState(currentState, startPoint, finishPoint);
            Log($"Quest state changed to {currentState} for quest '{questInfo.DisplayName}'");
        }

        private void Log(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[QuestPoint | {name}]: {message}");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (questInfo == null)
                Debug.LogWarning($"QuestPoint on GameObject '{gameObject.name}' has no QuestInfoSO assigned.", this);
        }
#endif
    }
}
